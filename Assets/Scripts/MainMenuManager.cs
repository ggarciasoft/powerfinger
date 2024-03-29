﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Assets.Scripts;
using Assets.Scripts.OnlineServices;
using PlayFab;

public class MainMenuManager : CommonManager
{
    #region Properties
    public GameObject[] btnPrincipalGameButton;
    private Text _txtBlockImageText, _txtServerStatus;
    private InputField _txtNickname;
    private Text txtBlockImageText
    {
        get
        {
            if (_txtBlockImageText == null)
                _txtBlockImageText = GameObject.Find("txtBlockImage").GetComponent<Text>();
            return _txtBlockImageText;
        }

        set
        {
            _txtBlockImageText = value;
        }
    }
    private Text txtServerStatus
    {
        get
        {
            if (_txtServerStatus == null)
                _txtServerStatus = GameObject.Find("txtServerStatus").GetComponent<Text>();
            return _txtServerStatus;
        }

        set
        {
            _txtServerStatus = value;
        }
    }
    private InputField txtNickname
    {
        get
        {
            if (_txtNickname == null)
                _txtNickname = GameObject.Find("txtNickname").GetComponent<InputField>();
            return _txtNickname;
        }

        set
        {
            _txtNickname = value;
        }
    }

    #endregion

    private void Start()
    {
        base.Init();
        Screen.sleepTimeout = 30;

        "pnlSettings".Hide();
        "pnlShop".Hide();

        System.Action<bool> init = (val) =>
        {
            if (val)
            {
                if (BackFromGame)
                {
                    InvokeRepeating("HideBlockImageAndText", 0, 0.01f);
                    BackFromGame = false;
                }
                else
                    InvokeRepeating("ShowTextBlockImage", 0, 0.01f);
                InitializeServer();
                UpdateServerService = true;
                SetCurrency();
            }
            else
                MessageBox("Login failed", onClose: () => Application.Quit());
        };

        if (MainOnlineService.AuthService.IsLogged())
            init(true);
        else
            MainOnlineService.AuthService.SignIn(init);
    }

    private void SetCurrency()
    {
        PlayFabClientAPI.GetUserInventory(new PlayFab.ClientModels.GetUserInventoryRequest(),
            (result) =>
            {
                GameObject.Find("lblOrbs").GetComponent<Text>().text = "x "+result.VirtualCurrency["OR"].ToString();
            },
            (error) =>
            {
                MessageBox("Error getting currency - " + error.HttpCode);
                Debug.LogError(error.ErrorMessage);
            });
    }

    private void ShowTextBlockImage()
    {
        if (txtBlockImageText.color.a >= 1f)
        {
            CancelInvoke("ShowTextBlockImage");
            InvokeRepeating("HideBlockImageAndText", 1.0f, 0.01f);
            return;
        }

        txtBlockImageText.color = new Color(0, 0, 0, txtBlockImageText.color.a + 0.01f);
    }

    private void HideBlockImageAndText()
    {
        if (BlockImage.color.a <= 0f)
        {
            CancelInvoke("HideBlockImageAndText");
            InvokeRepeating("GeneratePoint", 0f, 1f);
            DestroyImmediate(txtBlockImageText);
            BlockImageSetActive(false);
            return;
        }

        if (txtBlockImageText.color.a > 0f)
            txtBlockImageText.color = new Color(0, 0, 0, txtBlockImageText.color.a - 0.01f);

        BlockImage.color = new Color(255, 255, 255, BlockImage.color.a - 0.01f);
    }

    private void InitializeServer()
    {
        OnlineService.Initialize();
    }

    private void Update()
    {
        //GameObject.Find("btnJoinGame").GetComponent<Button>().interactable =
        //GameObject.Find("btnNewGame").GetComponent<Button>().interactable = false;

        //if (!UpdateServerService) return;

        //var connectionStatus = OnlineService.GetConnectionStatus();

        //txtServerStatus.text = connectionStatus.ToString();

        //if (connectionStatus == OnlineStatus.Connected)
        //    GameObject.Find("btnJoinGame").GetComponent<Button>().interactable =
        //    GameObject.Find("btnNewGame").GetComponent<Button>().interactable = true;

        //OnlineService.Sync();
    }
}
