using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.OnlineServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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

    private void OnlineServiceMessage(string msg, bool isError)
    {
        MessageBox(msg);
    }

    #region Main Menu Buttons

    public void ClickNewGameAutomatch()
    {
        IsJoining = false;
        OnlineService.SetOnShowWaitingRoomEvent(null);
        OnlineService.SetMessageEvent(OnlineServiceMessage);
        var join = OnlineService.CreateRoom();
        if (!join) return;
        ShowWaitingRoom();
    }

    public void ClickNewGameInvitation()
    {
        IsJoining = false;
        OnlineService.SetOnShowWaitingRoomEvent(null);
        OnlineService.SetMessageEvent(OnlineServiceMessage);
        var join = OnlineService.CreateRoom();
        if (!join) return;
        ShowWaitingRoom();
    }

    public void ClickViewInvitation()
    {
        UpdateServerService = false;
        IsJoining = true;
        OnlineService.SetOnShowWaitingRoomEvent(null);
        OnlineService.SetMessageEvent(OnlineServiceMessage);
        SetEnableFadeWhiteButtons(false);
        var join = OnlineService.InviteRoom();

        if (!join) return;
        OnlineService.SetOnShowWaitingRoomEvent(ShowWaitingRoom);
        Invoke("SetEnableFadeWhiteButtons", 1f);
    }

    private void ShowWaitingRoom()
    {
        SetEnableFadeWhiteButtons(false);
        ActionWhenBlockImageToggle = () =>
        {
            SceneManager.LoadScene("PlayboardOnlineVersus");
        };

        InvokeRepeating("ShowBlockImage", 0, 0.01f);
    }

    private void SetEnableFadeWhiteButtons(bool enable = true)
    {
        foreach (var item in GameObject.FindGameObjectsWithTag("FadeWhite"))
        {
            var btn = item.GetComponent<Button>();
            btn.interactable = enable;
        }
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

    public void ClickShop()
    {
        PlayFab.PlayFabClientAPI.GetCatalogItems(new PlayFab.ClientModels.GetCatalogItemsRequest(),
            (result) =>
            {
                PlayFab.PlayFabClientAPI.GetUserInventory( new PlayFab.ClientModels.GetUserInventoryRequest(),
            (result2) =>
            {
                foreach (var item in result.Catalog)
                {
                    var quantity = result2.Inventory.Where(o => o.ItemId == item.ItemId).Sum(o => o.RemainingUses);

                    var pnlItem = Instantiate(
                    Resources.LoadAll("pnlItem")[0],
                    GameObject.Find("ScrollViewItems").transform, false) as GameObject;
                    pnlItem.transform.Find("lblItemLetter").GetComponent<Text>().text = item.ItemId;
                    pnlItem.transform.Find("lblDescription").GetComponent<Text>().text = item.Description;
                    pnlItem.transform.Find("lblItemPrice").GetComponent<Text>().text = item.VirtualCurrencyPrices["OR"].ToString();
                    pnlItem.transform.Find("lblQuantity").GetComponent<Text>().text = quantity.GetValueOrDefault().ToString();
                }
                var rect = GameObject.Find("pnlItems").GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, (result.Catalog.Count * 100) + 100);

                "pnlMainMenu".Hide();
                "pnlShop".Show();
            },
            (error) =>
            {
                MessageBox("Error getting shop information");
            });
            },
            (error) =>
            {
                MessageBox("Error getting shop information");
            });
    }

    public void ClickBackShop()
    {
        "pnlMainMenu".Show();
        "pnlShop".Hide();
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

    public void ClickReviewConfiguration()
    {
        MessageBox(AuthenticationService.Instance.ReviewConfiguration());
    }
    #endregion
}
