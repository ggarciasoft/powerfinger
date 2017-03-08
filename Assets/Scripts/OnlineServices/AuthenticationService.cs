using GooglePlayGames;
using GooglePlayGames.BasicApi;
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
            /*var config = new
            PlayGamesClientConfiguration.Builder()
            .Build();

            // Enable debugging output (recommended)
            PlayGamesPlatform.DebugLogEnabled = true;

            // Initialize and activate the platform
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.Activate();*/
        }

        public bool IsLogged()
        {
            return false;//Social.localUser.authenticated;// PlayGamesPlatform.Instance.localUser.authenticated;
        }

        /// <summary>
        /// Call Authenticate Silent, if fail, call UI.
        /// </summary>
        /// <param name="callBack"></param>
        public void SignIn(Action<bool> callBack)
        {
            Social.localUser.Authenticate(callBack);
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
