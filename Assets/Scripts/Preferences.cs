using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Preferences
    {
        public static string RegionName
        {
            get
            {
                var regionName = PlayerPrefs.GetString("RegionName");
                if (String.IsNullOrEmpty(regionName))
                {
                    regionName = "us";
                    PlayerPrefs.SetString("RegionName", regionName);
                }
                return regionName;
            }
            set
            {
                var regionName = value;
                if (String.IsNullOrEmpty(regionName))
                    regionName = "us";

                PlayerPrefs.SetString("RegionName", regionName);
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

        public static string PlayFabId
        {
            get
            {
                return PlayerPrefs.GetString("PlayFabId");
            }
            set
            {
                PlayerPrefs.SetString("PlayFabId", value);
            }
        }
    }
}
