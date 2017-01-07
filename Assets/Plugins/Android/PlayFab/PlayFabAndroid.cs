﻿using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using PlayFab.Internal;
using PlayFab;

namespace PlayFab
{
    public class PlayFabAndroidPlugin
    {
        private static bool Initted = false;

#if UNITY_ANDROID //&& !UNITY_EDITOR
        private static AndroidJavaClass AndroidPlugin;
		private static AndroidJavaClass PlayServicesUtils;
        private static AndroidJavaClass NotificationSender;
#endif

        public static void Init(string SenderID)
        {
            Debug.LogFormat("SenderID: {0}", SenderID);
            if (Initted)
            {
                Debug.LogFormat("Android has already been initialized");
                return;
            }

            PlayFabPluginEventHandler.Init();

#if UNITY_ANDROID //&& !UNITY_EDITOR
            AndroidPlugin = new AndroidJavaClass("com.playfab.unityplugin.PlayFabUnityAndroidPlugin");
			string applicationName = "PlayFab Application";
#if UNITY_5 || UNITY_5_1
				applicationName = Application.productName;
#endif
			var staticParams = new object[] { SenderID , applicationName};
            AndroidPlugin.CallStatic("initGCM", staticParams);
            PlayServicesUtils = new AndroidJavaClass("com.playfab.unityplugin.GCM.PlayServicesUtils");
            NotificationSender = new AndroidJavaClass("com.playfab.unityplugin.GCM.PlayFabNotificationSender");
#endif
            PlayFabGoogleCloudMessaging.Init();

            Initted = true;
        }

#if UNITY_ANDROID //&& !UNITY_EDITOR

        public static bool IsPlayServicesAvailable()
		{
			return PlayServicesUtils.CallStatic<bool> ("isPlayServicesAvailable");
		}

        public static void StopPlugin(){
            AndroidPlugin.CallStatic("stopPluginService");
        }

		public static void UpdateRouting(bool routeToNotificationArea){
			AndroidPlugin.CallStatic("updateRouting", routeToNotificationArea);
        }

        public static void ScheduleNotification(string notification, DateTime date)
        {
            AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");

            var package = NotificationSender.CallStatic<AndroidJavaObject>("createNotificationPackage", new object[]
            {
                objActivity,
                notification
            });

            var dateString = date.ToString("MM-dd-yyyy HH:mm:ss");
            Debug.LogFormat("Setting Scheduled Date: {0}", dateString);
            package.Call("SetScheduleDate", dateString);
            
            NotificationSender.CallStatic("Send", new object[]
            {
                objActivity,
                package
            });
        }

        public static void SendNotificationNow(string notification)
        {
            AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
            
            //AndroidJavaObject package = new AndroidJavaObject("com.playfab.unityplugin.GCM.PlayFabNotificationPackage");
            var package = NotificationSender.CallStatic<AndroidJavaObject>("createNotificationPackage", new object[]
            {
                objActivity,
                notification
            });

            NotificationSender.CallStatic("Send", new object[]
            {
                objActivity,
                package
            });
        }

        public static void CancelNotification(string notification)
        {
            AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
            var package = NotificationSender.CallStatic<AndroidJavaObject>("createNotificationPackage", new object[]
            {
                objActivity,
                notification
            });
            NotificationSender.CallStatic("CancelScheduledNotification", new object[]
            {
                objActivity,
                package
            });
        }

#else
        public static bool IsPlayServicesAvailable()
        {
            return false;
        }
        
		public static void UpdateRouting(bool routeToNotificationArea)
		{

        }

        public static void ScheduleNotification(PlayFabNotificationPackage notification)
        {
            
        }

        public static void SendNotificationNow(PlayFabNotificationPackage notification)
        {
            
        }

        public static void CancelNotification(PlayFabNotificationPackage notification)
        {

        }

#endif
    }

    public class PlayFabGoogleCloudMessaging
    {
        #region Events
        public delegate void GCMRegisterReady(bool status);
        public delegate void GCMRegisterComplete(string id, string error);
        public delegate void GCMMessageReceived(string message);

        public static GCMRegisterReady _RegistrationReadyCallback;
        public static GCMRegisterComplete _RegistrationCallback;
        public static GCMMessageReceived _MessageCallback;
        #endregion
#if UNITY_ANDROID //&& !UNITY_EDITOR

        private static AndroidJavaClass PlayFabGCMClass;
        private static AndroidJavaClass PlayFabPushCacheClass;

		public static void Init()
		{
			PlayFabGCMClass = new AndroidJavaClass("com.playfab.unityplugin.GCM.PlayFabGoogleCloudMessaging");
            PlayFabPushCacheClass = new AndroidJavaClass("com.playfab.unityplugin.GCM.PlayFabPushCache");
		}

		public static void GetToken()
		{
			PlayFabGCMClass.CallStatic("getToken");
		}

        public static void ClearPushCache()
        {
            PlayFabPushCacheClass.CallStatic("clearPushCache");
        }

        public static string GetPushCacheData(){
            return PlayFabPushCacheClass.CallStatic<String>("getPushCacheData");
        }

		public static List<PlayFabNotificationPackage> GetPushCache()
        {
            var pushCache = new List<PlayFabNotificationPackage>();
            var packages = PlayFabPushCacheClass.CallStatic<List<AndroidJavaObject>>("getPushCache");

            if (packages != null)
			{
			    foreach (var package in packages)
			    {
                    var cachePackage = new PlayFabNotificationPackage();
                    cachePackage.Id = package.Get<string>("Id");
                    cachePackage.ScheduleType = package.Get<ScheduleTypes>("ScheduleType");
                    cachePackage.ScheduleDate = package.Get<DateTime>("ScheduleDate");
                    cachePackage.Title = package.Get<string>("Title");
                    cachePackage.Message = package.Get<string>("Message");
                    cachePackage.Icon = package.Get<string>("Icon");
                    cachePackage.Sound = package.Get<string>("Sound");
                    cachePackage.CustomData = package.Get<string>("CustomData");
                    cachePackage.Delivered = package.Get<bool>("Delivered");
                    pushCache.Add(cachePackage);
			    }
			}
			else
			{
				Debug.Log("Package was null");
			}
			
			return pushCache;
        }

#else

        public static void Init()
        {
			
        }

        public static string GetToken()
        {
            return null;
        }

        public static string GetPushCacheData()
        {
            return null;
        }

		public static PlayFabNotificationPackage GetPushCache()
        {
            return new PlayFabNotificationPackage();
        }
#endif

        internal static void RegistrationReady(bool status)
        {
            if (_RegistrationReadyCallback == null)
                return;

            _RegistrationReadyCallback(status);
            _RegistrationReadyCallback = null;
        }

        internal static void RegistrationComplete(string id, string error)
        {
            if (_RegistrationCallback == null)
                return;

            _RegistrationCallback(id, error);
            _RegistrationCallback = null;
        }

        internal static void MessageReceived(string message)
        {
            if (_MessageCallback == null)
                return;

            _MessageCallback(message);
        }

    }
}


// c# wrapper that matches our native com.playfab.unityplugin.GCM.PlayFabNotificationPackage
[Serializable]
public class PlayFabNotificationPackage
{
    public DateTime ScheduleDate;
    public ScheduleTypes ScheduleType;
    public string Sound;                // do not set this to use the default device sound; otherwise the sound you provide needs to exist in Android/res/raw/_____.mp3, .wav, .ogg
	public string Title;                // title of this message
	public string Icon;                 // to use the default app icon use app_icon, otherwise send the name of the custom image. Image must be in Android/res/drawable/_____.png, .jpg
	public string Message;              // the actual message to transmit (this is what will be displayed in the notification area
	public string CustomData;           // arbitrary key value pairs for game specific usage
    public string Id;
    public Boolean Delivered;
}

public enum ScheduleTypes
{
    None,
    ScheduledDate
}

