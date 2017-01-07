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
                return PlayerPrefs.GetString("Nickname");
            }
            set
            {
                PlayerPrefs.SetString("Nickname", value);
            }
        }

        public static float MusicVolume
        {
            get
            {
                return PlayerPrefs.GetFloat("MusicVolume");
            }
            set
            {
                PlayerPrefs.GetFloat("MusicVolume", value);
            }
        }

        public static float SoundVolume
        {
            get
            {
                return PlayerPrefs.GetFloat("SoundVolume");
            }
            set
            {
                PlayerPrefs.GetFloat("SoundVolume", value);
            }
        }
    }
}
