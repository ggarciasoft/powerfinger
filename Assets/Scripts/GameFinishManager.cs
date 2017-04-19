using Assets.Scripts;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class GameFinishManager : CommonManager
{
    private static GameFinishData _data;
    public static GameFinishData Data
    {
        get
        {
            if (_data == null)
                _data = new GameFinishData();
            return _data;
        }
        set
        {
            _data = Data;
        }
    }
    // Use this for initialization
    private void Start()
    {
        base.Init();

        InvokeRepeating("HideBlockImage", 0, 0.01f);
        InvokeRepeating("GeneratePoint", 0f, 1f);

        var lblTitle = GameObject.Find("lblTitle").GetComponent<Text>();

            var selfResult = (short)Data
            .ListGamePoints
            .Where(o => o.IsExplodeBySelf.HasValue && o.IsExplodeBySelf.Value)
            .Sum(o => o.RealValuePoint);
        var otherResult = (short)Data
            .ListGamePoints
            .Where(o => o.IsExplodeBySelf.HasValue && !o.IsExplodeBySelf.Value)
            .Sum(o => o.RealValuePoint);

        GameObject.Find("lblPointP1").GetComponent<Text>().text = Data
            .ListGamePoints
            .Where(o => o.IsExplodeBySelf.HasValue && o.IsExplodeBySelf.Value && o.Type == (short)GamePoint.GamePointType.NormalPoint)
            .Count().ToString();
        GameObject.Find("lblPointP2").GetComponent<Text>().text = Data
            .ListGamePoints
            .Where(o => o.IsExplodeBySelf.HasValue && !o.IsExplodeBySelf.Value && o.Type == (short)GamePoint.GamePointType.NormalPoint)
            .Count().ToString();
        GameObject.Find("lblSPointP1").GetComponent<Text>().text = Data
            .ListGamePoints
            .Where(o => o.IsExplodeBySelf.HasValue && o.IsExplodeBySelf.Value && o.Type == (short)GamePoint.GamePointType.SpecialPoint)
            .Count().ToString();
        GameObject.Find("lblSPointP2").GetComponent<Text>().text = Data
            .ListGamePoints
            .Where(o => o.IsExplodeBySelf.HasValue && !o.IsExplodeBySelf.Value && o.Type == (short)GamePoint.GamePointType.SpecialPoint)
            .Count().ToString();

        GameObject.Find("lblResultP1").GetComponent<Text>().text = selfResult.ToString();
        GameObject.Find("lblResultP2").GetComponent<Text>().text = otherResult.ToString();

        if (!Data.IsWinner.HasValue)
        {
            if (selfResult== otherResult)
            {
                lblTitle.text = "DRAW!!";
            }
            else
                Data.IsWinner = selfResult> otherResult;
        }

        lblTitle.text = Data.IsWinner.GetValueOrDefault() ? "WINNER!!" : "LOSER!!";

    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).tapCount == 2)
        {
            Data = null;
            BackFromGame = true;
            SceneManager.LoadScene("MainMenu");
            return;
        }
    }

}
