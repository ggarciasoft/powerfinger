﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Assets.Scripts;
using Assets.Scripts.OnlineServices;

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

    #region Main Menu Buttons

    public void ClickNewGame()
    {
        "pnlMainMenu".Hide();
        "pnlNewGame".Show();
    }

    public void ClickJoinGame()
    {
        UpdateServerService = false;
        var join = OnlineService.JoinRoom();

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

    public void ClickMainMenuSettings()
    {
        GameObject.Find("sldMusic").GetComponent<Slider>().value = Preferences.MusicVolume;
        GameObject.Find("sldSound").GetComponent<Slider>().value = Preferences.SoundVolume;
        "pnlMainMenu".Hide();
        "pnlSettings".Show();
    }

    public void ClickLeaderboard()
    {
        Social.ShowLeaderboardUI();
    }
    #endregion

    #region Settings

    public void ClickSaveSettings()
    {
        var music = GameObject.Find("sldMusic").GetComponent<Slider>();
        var sound = GameObject.Find("sldSound").GetComponent<Slider>();
        Preferences.MusicVolume = music.value;
        Preferences.SoundVolume = sound.value;
        MessageBox("Settings saved successfully!");
    }

    public void ClickMainMenuBackSettings()
    {
        "pnlSettings".Hide();
        "pnlMainMenu".Show();
    }
    #endregion

    #region New Game

    public void ClickMainMenuBackNewGame()
    {
        "pnlNewGame".Hide();
        "pnlMainMenu".Show();
    }

    public void sldTimeChanged(float value)
    {
        GameObject.Find("txtTime").GetComponent<Text>().text = value.ToString();
    }

    public void ClickInvite()
    {
        var join = OnlineService.InviteRoom();

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

    public void ClickAutomatch()
    {
        var join = OnlineService.CreateRoom();

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

    #endregion

    public void ClickReviewConfiguration()
    {
        MessageBox(AuthenticationService.Instance.ReviewConfiguration());
    }

    #endregion
}
