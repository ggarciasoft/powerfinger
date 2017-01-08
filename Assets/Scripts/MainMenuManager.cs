using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Assets.Scripts;

public class MainMenuManager : CommonManager
{
    #region Properties
    public GameObject[] lstAvailablePoints, btnPrincipalGameButton;
    private List<GameObject> _lstInstatiatePoints;
    private float _camHalfHeight, _camHalfWidth;
    private Text _txtBlockImageText, _txtRegionName, _txtServerStatus;
    private InputField _txtNickname;
    private Text txtBlockImageText
    {
        get
        {
            if(_txtBlockImageText == null)
                _txtBlockImageText = GameObject.Find("txtBlockImage").GetComponent<Text>();
            return _txtBlockImageText;
        }

        set
        {
            _txtBlockImageText = value;
        }
    }
    private Text txtRegionName
    {
        get
        {
            if (_txtRegionName == null)
                _txtRegionName = GameObject.Find("txtRegionName").GetComponent<Text>();
            return _txtRegionName;
        }

        set
        {
            _txtRegionName = value;
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
        "pnlSettings".Hide();
        UpdateServerService = true;

        _camHalfHeight = Camera.main.orthographicSize;
        _camHalfWidth = Camera.main.aspect * _camHalfHeight;

        _lstInstatiatePoints = new List<GameObject>();

        txtNickname.text = Preferences.Nickname;

        InvokeRepeating("ShowTextBlockImage", 0, 0.01f);

        InitialiseServer();
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

        txtBlockImageText.color = new Color(0, 0, 0, txtBlockImageText.color.a - 0.01f);
        BlockImage.color = new Color(255, 255, 255, BlockImage.color.a - 0.01f);
    }

    private void GeneratePoint()
    {
        if (_lstInstatiatePoints.Count >= 6)
        {
            var obj = _lstInstatiatePoints.First();
            DestroyImmediate(obj);
            _lstInstatiatePoints.Remove(obj);
        }

        var randomPoint = Random.Range(0, lstAvailablePoints.Length);
        float x = Random.Range(-_camHalfWidth, _camHalfWidth), y = Random.Range(-_camHalfHeight, _camHalfHeight), z = 0;
        var point = lstAvailablePoints[randomPoint];
        point.ParticleColorOverLifetime(true);
        point = Instantiate(point, new Vector3(x, y, z), Quaternion.identity) as GameObject;
        _lstInstatiatePoints.Add(point);
    }

    private void InitialiseServer()
    {
        Client.ConnectToRegionMaster(RegionName);
        txtRegionName.text = "REGION: " + RegionName.ToUpper();
    }
    /*
    private void Update()
    {
        GameObject.Find("btnJoinGame").GetComponent<Button>().interactable =
        GameObject.Find("btnCreateGame").GetComponent<Button>().interactable = false;

        if (!UpdateServerService) return;

        switch (Client.State)
        {
            case ExitGames.Client.Photon.LoadBalancing.ClientState.Uninitialized:
                txtServerStatus.text = "UNINITIALIZED";
                break;
            case ExitGames.Client.Photon.LoadBalancing.ClientState.ConnectingToGameserver:
            case ExitGames.Client.Photon.LoadBalancing.ClientState.ConnectedToGameserver:
            case ExitGames.Client.Photon.LoadBalancing.ClientState.Joining:
            case ExitGames.Client.Photon.LoadBalancing.ClientState.Joined:
            case ExitGames.Client.Photon.LoadBalancing.ClientState.ConnectingToMasterserver:
            case ExitGames.Client.Photon.LoadBalancing.ClientState.ConnectedToMaster:
            case ExitGames.Client.Photon.LoadBalancing.ClientState.ConnectedToNameServer:
            case ExitGames.Client.Photon.LoadBalancing.ClientState.ConnectingToNameServer:
                txtServerStatus.text = "CONNECTING";
                break;
            case ExitGames.Client.Photon.LoadBalancing.ClientState.JoinedLobby:
                GameObject.Find("btnJoinGame").GetComponent<Button>().interactable =
                GameObject.Find("btnCreateGame").GetComponent<Button>().interactable = true;
                txtServerStatus.text = "CONNECTED";
                break;
            case ExitGames.Client.Photon.LoadBalancing.ClientState.DisconnectingFromMasterserver:
            case ExitGames.Client.Photon.LoadBalancing.ClientState.Leaving:
            case ExitGames.Client.Photon.LoadBalancing.ClientState.DisconnectingFromGameserver:
            case ExitGames.Client.Photon.LoadBalancing.ClientState.Disconnecting:
            case ExitGames.Client.Photon.LoadBalancing.ClientState.DisconnectingFromNameServer:
                txtServerStatus.text = "DISCONNECTING";
                break;
            case ExitGames.Client.Photon.LoadBalancing.ClientState.Disconnected:
                txtServerStatus.text = "DISCONNECTED";
                break;
            default:
                txtServerStatus.text = "UNKNOWN";
                break;
        }

        Client.NickName = txtPlayerName.text;

        PlayerPrefs.SetString("PlayerName", txtPlayerName.text);

        Client.Service();
    }*/
}
