using Assets.Scripts.OnlineServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

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
            var countMessageBox = GameObject.FindGameObjectsWithTag("MessageBox").Length;

            var pnlMessageBox = Instantiate(
                Resources.LoadAll("pnlMessageBox")[0],
                GameObject.Find("Canvas").transform, false) as GameObject;

            if (countMessageBox == 0)
            {
                pnlMessageBox.transform.localScale = new Vector3(0, 0, 1);
                pnlMessageBox.name = "pnlMessageBox";
                _limitScale = 1.2m;
                _amountToSumScale = 0.1m;
                InvokeRepeating("MessageBoxAnimation", 0.2f, 0.009f);
            }
            else
                pnlMessageBox.name = "pnlMessageBox" + countMessageBox;

            pnlMessageBox.transform.Find("Panel/lblMessage").GetComponent<Text>().text = message;

            if (onOk != null)
                pnlMessageBox.transform.Find("Panel/Panel/btnOk").GetComponent<Button>().onClick.AddListener(() => onOk());
            else
                pnlMessageBox.transform.Find("Panel/Panel/btnOk").gameObject.SetActive(false);

            if (onClose != null)
                pnlMessageBox.transform.Find("Panel/Panel/btnClose").GetComponent<Button>().onClick.AddListener(() => onClose());

            pnlMessageBox.transform.Find("Panel/Panel/btnClose").GetComponent<Button>().onClick.AddListener(() => ClickMessageBoxClose(pnlMessageBox));
        }

        private void MessageBoxAnimation()
        {
            var pnlMessageBox = GameObject.Find("pnlMessageBox");
            if (pnlMessageBox == null)
            {
                CancelInvoke("MessageBoxAnimation");
                return;
            }

            var scale = Convert.ToDecimal(pnlMessageBox.transform.localScale.x);
            
            if (scale == _limitScale)
            {
                if (_limitScale == 1.2m)
                {
                    _limitScale = 0.8m;
                    _amountToSumScale = -0.1m;
                }
                else if (_limitScale == 0.8m)
                {
                    _limitScale = 1m;
                    _amountToSumScale = 0.1m;
                }
                else
                {
                    CancelInvoke("MessageBoxAnimation");
                    return;
                }
            }

            scale += _amountToSumScale;
            pnlMessageBox.transform.localScale = new Vector3((float)scale, (float)scale, 1);
        }

        private decimal _limitScale, _amountToSumScale;

        public void ClickMessageBoxClose(GameObject pnlMessageBox)
        {
            DestroyImmediate(pnlMessageBox);
        }
        #endregion
    }
}
