using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Assets.Scripts;

public class PlayboardLocalVersusManager : CommonManager
{
    public List<GameObject> lstPrefabPoints;

    private Dictionary<GamePoint.GamePointType, GameObject> _listGamePointsType;
    private List<GamePoint> _listGamePointsGenerated;
    //private GameObject _playboard;
    private Text _txtTimer;
    private float _timerCount = 30f;

    void Start()
    {
        //_playboard = GameObject.Find("Playboard");
        _txtTimer = GameObject.Find("txtTimer").GetComponent<Text>();
        _listGamePointsType = new Dictionary<GamePoint.GamePointType, GameObject>();
        _listGamePointsGenerated = new List<GamePoint>();
        SettxtTimer();

        foreach (var item in lstPrefabPoints)
        {
            _listGamePointsType.Add(
                (GamePoint.GamePointType)
                System.Enum.Parse(typeof(GamePoint.GamePointType), item.name), item);
        }

        lstPrefabPoints.Clear();

        for (int i = 3; i > 0; i--)
        {
            lstPrefabPoints.Add(GameObject.Find("CountDownPoint" + i));
        }

        var randomPointCount = Random.Range(_timerCount, _timerCount * 2);

        for (int i = 0; i < randomPointCount; i++)
        {
            var randomType = Random.Range(0, 100) <= 10 ? GamePoint.GamePointType.SpecialPoint : GamePoint.GamePointType.NormalPoint;

            _listGamePointsGenerated.Add(new GamePoint
            {
                Type = randomType,
                Time = Random.Range(0f, _timerCount),
                IsFirstPlayer = true
            });

            _listGamePointsGenerated.Add(new GamePoint
            {
                Type = randomType,
                Time = Random.Range(0f, _timerCount),
            });
        }

        ActionWhenBlockImageToggle = () =>
        {
            BlockImageSetActive(true);
            InvokeRepeating("CountDownInit", 2.0f, 1.0f);
        };

        InvokeRepeating("HideBlockImage", 0, 0.01f);
    }

    private void CountDownInit()
    {
        if (!lstPrefabPoints.Any())
        {
            CancelInvoke("CountDownInit");
            InvokeRepeating("UpdateGame", 0f, 0.01f);
            BlockImageSetActive(false);
            return;
        }

        var countDownPoint = lstPrefabPoints.First();

        var temp = _listGamePointsType[GamePoint.GamePointType.PointExplodeSpread];
        temp.ParticleColorOverLifetime(false);
        var _temp = Instantiate(
            _listGamePointsType[GamePoint.GamePointType.PointExplodeSpread],
            new Vector3(
                countDownPoint.transform.position.x,
                countDownPoint.transform.position.y,
                countDownPoint.transform.position.z),
            Quaternion.identity) as GameObject;

        var particle = _temp.GetComponent<ParticleSystem>().main;
        particle.simulationSpeed = 5f;
        DestroyImmediate(countDownPoint);

        Destroy(_temp, 1.5f);

        lstPrefabPoints.Remove(countDownPoint);
    }

    private void UpdateGame()
    {
        var lstPointToInstatiate = _listGamePointsGenerated.Where(o => o.Time >= _timerCount && !o.Instatiated);

        if (lstPointToInstatiate.Any())
        {
            foreach (var item in lstPointToInstatiate)
            {
                var point = _listGamePointsType[item.Type];
                var particle = point.GetComponent<ParticleSystem>().main;

                point.ParticleColorOverLifetime(item.IsFirstPlayer ? "#6A6BFFFF" : "#FF6776FF");

                float x = Random.Range(-6.72f, 6.72f), y = Random.Range(-2.9f, 5.82f), z = 0;

                item.Instatiated = true;

                if (item.Type == GamePoint.GamePointType.NormalPoint)
                    particle.simulationSpeed = 0.4f;

                point = Instantiate(point, new Vector3(x, y, z), Quaternion.identity) as GameObject;

                if (Input.GetMouseButtonDown(0) && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {


                    /*var touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    var touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);
                    var hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);

                    if (hitInformation.collider != null)
                    {
                        GameObject touchedObject = hitInformation.transform.gameObject;
                        Debug.Log("Touched " + touchedObject.transform.name);

                    }*/
                }
                //Destroy(point, particle.duration + 1);
            }

        }

        _timerCount -= 0.01f;
        SettxtTimer();
    }

    private void Update()
    {

    }

    private void SettxtTimer()
    {
        _txtTimer.text = _timerCount.ToString("0.00");
    }

    public class GamePoint
    {
        public GamePointType Type { get; set; }
        public bool IsFirstPlayer { get; set; }
        public float Time { get; set; }
        public bool Instatiated { get; set; }

        public enum GamePointType
        {
            NormalPoint,
            SpecialPoint,
            PointExplode,
            PointExplodeSpread
        }
    }
}
