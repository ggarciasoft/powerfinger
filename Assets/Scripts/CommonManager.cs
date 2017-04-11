using Assets.Scripts.OnlineServices;
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


        protected IOnlineService OnlineService
        {
            get
            {
                return MainOnlineService.OnlineService;
            }
        }

        protected Action ActionWhenBlockImageToggle { get; set; }
        protected static bool IsFirstPlayer { get; set; }
        protected static bool UpdateServerService { get; set; }
        protected static bool BackFromGame { get; set; }
        protected static bool InitiateGame { get; set; }
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
                    ActionWhenBlockImageToggle.Invoke();
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
                    ActionWhenBlockImageToggle.Invoke();
                return;
            }
            BlockImage.color = new Color(255, 255, 255, BlockImage.color.a - 0.01f);
        }

        protected void MessageBox(string message, Action onOk = null, Action onClose = null)
        {
            var pnlMessageBox = Instantiate(
                Resources.LoadAll("pnlMessageBox")[0],
                GameObject.Find("Canvas").transform, false) as GameObject;
            pnlMessageBox.name = "pnlMessageBox";
            pnlMessageBox.transform.Find("lblMessage").GetComponent<Text>().text = message;

            if (onOk != null)
                pnlMessageBox.transform.Find("btnOk").GetComponent<Button>().onClick.AddListener(() => onOk());
            else
                Destroy(pnlMessageBox.transform.Find("btnOk"));

            if (onClose != null)
                pnlMessageBox.transform.Find("btnClose").GetComponent<Button>().onClick.AddListener(() => onClose());

            pnlMessageBox.transform.Find("btnClose").GetComponent<Button>().onClick.AddListener(ClickMessageBoxClose);
        }

        public void ClickMessageBoxClose()
        {
            var pnlMessageBox = GameObject.Find("pnlMessageBox");
            Destroy(pnlMessageBox);
        }
        #endregion
    }
}
