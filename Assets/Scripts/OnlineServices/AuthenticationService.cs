using GooglePlayGames;
using GooglePlayGames.BasicApi;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.OnlineServices
{
    public class AuthenticationService
    {
        private static AuthenticationService _instance;

        public static AuthenticationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AuthenticationService();
                }

                return _instance;
            }
        }

        public AuthenticationService()
        {
            var config = new
            PlayGamesClientConfiguration.Builder()
            .AddOauthScope("profile")
            .Build();

            // Enable debugging output (recommended)
            PlayGamesPlatform.DebugLogEnabled = true;

            // Initialize and activate the platform
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.Activate();
        }

        public bool IsLogged()
        {
            return Social.localUser.authenticated && PlayFabClientAPI.IsClientLoggedIn();// PlayGamesPlatform.Instance.localUser.authenticated;
        }
        
        public string ReviewConfiguration()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("GPGS Is Logged: " + Social.localUser.authenticated);
            sb.AppendLine("PlayFab Is Logged: " + PlayFabClientAPI.IsClientLoggedIn());
            return sb.ToString();
        }

        /// <summary>
        /// Call Authenticate Silent, if fail, call UI.
        /// </summary>
        /// <param name="callBack"></param>
        public void SignIn(Action<bool> callBack)
        {
            //PlayGamesPlatform.Instance.Authenticate(callBack, false);
            Social.Active.localUser.Authenticate((success) =>
            {
                if (success)
                {
                    PlayGamesPlatform.Instance.GetServerAuthCode((code, authToken) =>
                    {
                        Debug.LogError("Code Return ServerAuthCode: " + code);
                        PlayFabClientAPI.LoginWithGoogleAccount(new LoginWithGoogleAccountRequest()
                        {
                            TitleId = PlayFabSettings.TitleId,
                            ServerAuthCode = authToken,
                            CreateAccount = true
                        }, (successLoginResult) =>
                        {
                            Debug.LogFormat("Login With Google Success: ", successLoginResult.PlayFabId);
                            callBack(success);
                        }, (errorResult) =>
                        {
                            Debug.Log(errorResult.GenerateErrorReport());
                            callBack(success);
                        });
                    });
                }
            });
            /*
            Action<bool> innerCallBack = (val) =>
            {
                if (val)
                    callBack(true);
                else
                    PlayGamesPlatform.Instance.Authenticate(callBack, true);
            };

            PlayGamesPlatform.Instance.Authenticate(innerCallBack, false);*/
        }
    }
}
