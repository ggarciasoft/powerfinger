using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Preferences
    {
        public static string Nickname
        {
            get
            {
                var nickName = PlayerPrefs.GetString("Nickname");
                if (String.IsNullOrEmpty(nickName))
                {
                    nickName = "Guess" + Guid.NewGuid().ToString().Replace('-', ' ').Substring(0, 13);
                    PlayerPrefs.SetString("Nickname", nickName);
                }
                return nickName;
            }
            set
            {
                var nickName = value;
                if (String.IsNullOrEmpty(nickName))
                    nickName = "Guess" + Guid.NewGuid().ToString().Replace('-', ' ').Substring(0, 13);

                PlayerPrefs.SetString("Nickname", nickName);
            }
        }

        public static float MusicVolume
        {
            get
            {
                return PlayerPrefs.GetFloat("MusicVolume", 1);
            }
            set
            {
                PlayerPrefs.SetFloat("MusicVolume", value);
            }
        }

        public static float SoundVolume
        {
            get
            {
                return PlayerPrefs.GetFloat("SoundVolume", 1);
            }
            set
            {
                PlayerPrefs.SetFloat("SoundVolume", value);
            }
        }
    }
}
