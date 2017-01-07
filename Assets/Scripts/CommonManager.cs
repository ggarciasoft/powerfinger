using ExitGames.Client.Photon.LoadBalancing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Assets.Scripts
{
    public class CommonManager : MonoBehaviour
    {
        #region Properties
        private Image _blockImage;
        private string _regionName;
        protected Image BlockImage
        {
            get
            {
                if (_blockImage == null)
                {
                    BlockImageSetActive(true);
                    _blockImage = GameObject.Find("BlockImage").GetComponent<Image>();
                }
                return _blockImage;
            }
            set
            {
                _blockImage = value;
            }
        }
        protected string RegionName
        {
            get
            {
                if (String.IsNullOrEmpty(_regionName))
                {
                    _regionName = PlayerPrefs.GetString("RegionName", "us");
                }

                return _regionName;
            }
            set
            {
                PlayerPrefs.SetString("RegionName", value);
                _regionName = value;
            }
        }
        protected PowerFingerBalancingClient Client
        {
            get
            {
                return PowerFingerBalancingClient.Instance;
            }
        }
        protected bool IsFirstPlayer { get; set; }
        protected Action ActionWhenBlockImageToggle { get; set; }
        protected static bool UpdateServerService { get; set; }
        #endregion

        #region Methods
        protected void BlockImageSetActive(bool active)
        {
            GameObject.Find("Canvas").transform.Find("BlockImage").gameObject.SetActive(active);
        }

        private void ShowBlockImage()
        {
            if (BlockImage.color.a >= 1f)
            {
                CancelInvoke("ShowBlockImage");
                if (ActionWhenBlockImageToggle != null)
                    ActionWhenBlockImageToggle();
                return;
            }
            BlockImage.color = new Color(255, 255, 255, BlockImage.color.a + 0.01f);
        }

        private void HideBlockImage()
        {
            if (BlockImage.color.a <= 0f)
            {
                CancelInvoke("HideBlockImage");
                BlockImageSetActive(false);
                if (ActionWhenBlockImageToggle != null)
                    ActionWhenBlockImageToggle();
                return;
            }
            BlockImage.color = new Color(255, 255, 255, BlockImage.color.a - 0.01f);
        }
        #endregion

        public enum EventDataCode
        {
            PointExplode
        }

        public enum EventDataParameter
        {
            PointId,
            SumScore
        }
    }
}
