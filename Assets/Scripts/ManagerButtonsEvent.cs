using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Assets.Scripts;
using ExitGames.Client.Photon.LoadBalancing;

public class ManagerButtonsEvent : CommonManager
{
    #region Methods
    public void ExitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void ClickMessageBoxOk()
    {
        var pnlMessageBox = GameObject.Find("pnlMessageBox");
        Destroy(pnlMessageBox);
    }

    #region Main Menu Buttons

    public void ClickCreateGame()
    {
        IsFirstPlayer = true;
        var join = Client.OpCreateRoom("Room1", new RoomOptions
        {
            MaxPlayers = 2,
            EmptyRoomTtl = 2000,
            PlayerTtl = 2000,
            CheckUserOnJoin = true
        }, TypedLobby.Default);

        if (!join) return;

        foreach (var item in GameObject.FindGameObjectsWithTag("FadeWhite"))
        {
            var btn = item.GetComponent<Button>();
            btn.interactable = false;
        }

        ActionWhenBlockImageToggle = () =>
        {
            SceneManager.LoadScene("PlayboardOnlineVersus");
        };

        InvokeRepeating("ShowBlockImage", 0, 0.01f);
    }

    public void ClickJoinGame()
    {
        IsFirstPlayer = false;
        UpdateServerService = false;
        var join = Client.OpJoinRoom("Room1");

        if (!join) return;

        foreach (var item in GameObject.FindGameObjectsWithTag("FadeWhite"))
        {
            var btn = item.GetComponent<Button>();
            btn.interactable = false;
        }

        ActionWhenBlockImageToggle = () =>
        {
            SceneManager.LoadScene("PlayboardOnlineVersus");
        };

        InvokeRepeating("ShowBlockImage", 0, 0.01f);
    }

    public void ClickSaveSettings()
    {
        var nickname = GameObject.Find("txtNickname").GetComponent<InputField>();
        var music = GameObject.Find("sldMusic").GetComponent<Slider>();
        var sound = GameObject.Find("sldSound").GetComponent<Slider>();
        if (String.IsNullOrEmpty(nickname.text))
            nickname.text = Guid.NewGuid().ToString().Replace('-', ' ');
        nickname.text = nickname.text.Length > 10 ? nickname.text.Substring(0, 10) : nickname.text;

        Preferences.Nickname = nickname.text;
        Preferences.MusicVolume = music.value;
        Preferences.SoundVolume = sound.value;
        MessageBox("Settings saved successfully!");
    }

    public void ClickMainMenuSettings()
    {
        GameObject.Find("txtNickname").GetComponent<InputField>().text = Preferences.Nickname;
        GameObject.Find("sldMusic").GetComponent<Slider>().value = Preferences.MusicVolume;
        GameObject.Find("sldSound").GetComponent<Slider>().value = Preferences.SoundVolume;
        "pnlMainMenu".Hide();
        "pnlSettings".Show();
    }

    public void ClickMainMenuCancelSettings()
    {
        "pnlSettings".Hide();
        "pnlMainMenu".Show();
    }
    #endregion

    #endregion
}
