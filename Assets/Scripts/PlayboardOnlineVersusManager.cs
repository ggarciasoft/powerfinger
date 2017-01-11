using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Assets.Scripts;
using ExitGames.Client.Photon.LoadBalancing;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayboardOnlineVersusManager : CommonManager
{
    #region Properties
    public List<GameObject> lstPrefabPoints;

    private Dictionary<GamePoint.GamePointType, GameObject> _listGamePointsType;
    private List<GamePoint> _listGamePointsGenerated;
    private GameObject _playboard;
    private Text _txtTimer, _txtScoreP1, _txtScoreP2, _txtNameP1, _txtNameP2, _lblWaitingForPlayer;
    private float _timerCount = 30f;
    private bool _gameInitiate;
    private short _currentSelfScore, _currentOtherScore;

    private GameObject Playboard
    {
        get
        {
            if (_playboard == null)
                _playboard = GameObject.Find("Playboard");
            return _playboard;
        }
        set
        {
            _playboard = value;
        }
    }
    private Text TxtTimer
    {
        get
        {
            if (_txtTimer == null)
                _txtTimer = GameObject.Find("txtTimer").GetComponent<Text>();
            return _txtTimer;
        }
        set
        {
            _txtTimer = value;
        }
    }
    private Text TxtScoreP1
    {
        get
        {
            if (_txtScoreP1 == null)
                _txtScoreP1 = GameObject.Find("txtScoreP1").GetComponent<Text>();
            return _txtScoreP1;
        }
        set
        {
            _txtScoreP1 = value;
        }
    }
    private Text TxtScoreP2
    {
        get
        {
            if (_txtScoreP2 == null)
                _txtScoreP2 = GameObject.Find("txtScoreP2").GetComponent<Text>();
            return _txtScoreP2;
        }
        set
        {
            _txtScoreP2 = value;
        }
    }
    private Text TxtNameP1
    {
        get
        {
            if (_txtNameP1 == null)
                _txtNameP1 = GameObject.Find("txtNameP1").GetComponent<Text>();
            return _txtNameP1;
        }
        set
        {
            _txtNameP1 = value;
        }
    }
    private Text TxtNameP2
    {
        get
        {
            if (_txtNameP2 == null)
                _txtNameP2 = GameObject.Find("txtNameP2").GetComponent<Text>();
            return _txtNameP2;
        }
        set
        {
            _txtNameP2 = value;
        }
    }
    private Text LblWaitingForPlayer
    {
        get
        {
            if (_lblWaitingForPlayer == null)
                _lblWaitingForPlayer = GameObject.Find("lblWaitingForPlayer").GetComponent<Text>();
            return _lblWaitingForPlayer;
        }
        set
        {
            _lblWaitingForPlayer = value;
        }
    }
    #endregion

    #region Methods
    void Start()
    {
        UpdateServerService = true;
        Client.OnRoomFullAction += () =>
        {
            _gameInitiate = true;
            TxtNameP1.text = Client.CurrentRoom.Players.First().Value.NickName;
            TxtNameP2.text = Client.CurrentRoom.Players.Last().Value.NickName;
            InvokeRepeating("HideBlockImage", 0, 0.01f);
        };

        Client.OnPointExplodeAction += (pointExplode) =>
        {
            Debug.Log("Point Explode");
            PointExplodeFromOponent(pointExplode.PointId, pointExplode.SumScore);
        };

        Initialise();

        SetTimer();

        foreach (var item in lstPrefabPoints)
        {
            _listGamePointsType.Add(
                (GamePoint.GamePointType)
                System.Enum.Parse(typeof(GamePoint.GamePointType), item.name), item);
        }

        lstPrefabPoints.Clear();

        for (byte i = 3; i > 0; i--)
        {
            lstPrefabPoints.Add(GameObject.Find("CountDownPoint" + i));
        }

        var randomPointCount = Random.Range(_timerCount, _timerCount * 2);

        for (short i = 0; i < randomPointCount; i++)
        {
            var randomType = Random.Range(0, 100) <= 10 ? GamePoint.GamePointType.SpecialPoint : GamePoint.GamePointType.NormalPoint;

            _listGamePointsGenerated.Add(new GamePoint
            {
                Id = i,
                Type = randomType,
                Time = i
            });
        }

        ActionWhenBlockImageToggle = () =>
        {
            BlockImageSetActive(true);
            InvokeRepeating("CountDownInit", 2.0f, 1.0f);
        };

        InvokeRepeating("ShowWaitingForPlayer", 0, 0.01f);
    }

    private void Initialise()
    {
        Playboard = GameObject.Find("Playboard");
        TxtTimer = GameObject.Find("txtTimer").GetComponent<Text>();
        TxtScoreP1 = GameObject.Find("txtScoreP1").GetComponent<Text>();
        TxtScoreP2 = GameObject.Find("txtScoreP2").GetComponent<Text>();
        TxtNameP1 = GameObject.Find("txtNameP1").GetComponent<Text>();
        TxtNameP2 = GameObject.Find("txtNameP2").GetComponent<Text>();
        _listGamePointsType = new Dictionary<GamePoint.GamePointType, GameObject>();
        _listGamePointsGenerated = new List<GamePoint>();
    }

    private void ShowWaitingForPlayer()
    {
        if (LblWaitingForPlayer.color.a >= 1)
        {
            CancelInvoke("ShowWaitingForPlayer");
            InvokeRepeating("HideWaitingForPlayer", 0, 0.01f);
            return;
        }
        LblWaitingForPlayer.color = new Color(0, 0, 0, LblWaitingForPlayer.color.a + 0.01f);
    }

    private void HideWaitingForPlayer()
    {
        if (LblWaitingForPlayer.color.a <= 0)
        {
            CancelInvoke("HideWaitingForPlayer");
            if (!_gameInitiate)
                InvokeRepeating("ShowWaitingForPlayer", 0, 0.01f);
            return;
        }
        LblWaitingForPlayer.color = new Color(0, 0, 0, LblWaitingForPlayer.color.a - 0.01f);
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
        ExplodePoint(countDownPoint);

        lstPrefabPoints.Remove(countDownPoint);
    }

    private void ExplodePoint(GameObject pointToExplode)
    {
        var _temp = Instantiate(
           _listGamePointsType[GamePoint.GamePointType.PointExplode],
           new Vector3(
               pointToExplode.transform.position.x,
               pointToExplode.transform.position.y,
               pointToExplode.transform.position.z),
           Quaternion.identity) as GameObject;

        var particle = _temp.GetComponent<ParticleSystem>();
        particle.playbackSpeed = 2f;
        DestroyImmediate(pointToExplode);

        Destroy(_temp, 1.5f);
    }

    private void UpdateGame()
    {
        if (_timerCount <= 0)
        {
            CancelInvoke("UpdateGame");
            BlockImageSetActive(true);
            return;
        }

        var lstPointToInstatiate = _listGamePointsGenerated.Where(o => o.Time >= _timerCount && !o.Instatiated);

        if (lstPointToInstatiate.Any())
        {
            foreach (var item in lstPointToInstatiate)
            {
                var point = _listGamePointsType[item.Type];
                var particle = point.GetComponent<ParticleSystem>();

                float x = Random.Range(-6.72f, 6.72f), y = Random.Range(-2.9f, 5.82f), z = 0;

                item.Instatiated = true;

                if (item.Type == GamePoint.GamePointType.NormalPoint)
                    particle.playbackSpeed = 0.4f;

                point = Instantiate(point, new Vector3(x, y, z), Quaternion.identity) as GameObject;
                point.name = "Point-" + item.Id;
                Destroy(point, particle.duration + 1);
            }
        }

        _timerCount -= 0.01f;
        SetTimer();
    }

    private void Update()
    {
        Client.Service();

        //Debug.Log("Current Status:" + Client.State.ToString());

        if (Client.CurrentRoom == null) return;

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        GameObject objectTouched = null;
        var isTouched = Input.GetMouseButtonDown(0) || Input.touchCount > 0;

        if (!isTouched) return;

        System.Action<Vector3> actionObjectTouched = (position) =>
        {
            var ray = Camera.main.ScreenToWorldPoint(position);

            var raycast = Physics2D.LinecastAll(ray, ray).FirstOrDefault();

            if (raycast.collider == null) return;

            if (!(raycast.collider.CompareTag("NormalPoint") || raycast.collider.CompareTag("SpecialPoint"))) return;

            objectTouched = raycast.collider.gameObject;

            var sumScore = byte.MinValue;

            if (raycast.collider.CompareTag("NormalPoint"))
                sumScore++;
            else if (raycast.collider.CompareTag("SpecialPoint"))
                sumScore += 5;

            _currentSelfScore += sumScore;
            var id = short.Parse(objectTouched.name.Split('-')[1]);
            Hashtable evData = new Hashtable();
            evData[(byte)EventDataParameter.PointId] = id;
            evData[(byte)EventDataParameter.SumScore] = sumScore;

            Client.OpRaiseEvent((byte)EventDataCode.PointExplode, evData, true, RaiseEventOptions.Default);

            ExplodePoint(objectTouched);
        };

        if (Input.GetMouseButtonDown(0))
            actionObjectTouched(Input.mousePosition);
        else
            for (int i = 0; i < Input.touchCount; i++)
            {
                var touch = Input.GetTouch(i);
                if (touch.phase != TouchPhase.Began) continue;
                actionObjectTouched(touch.position);
            }

        SetSelfScore();
    }

    private void PointExplodeFromOponent(short pointId, byte sumScore)
    {
        //var point = _listGamePointsGenerated.Single(o => o.Id == pointId);
        var objectTouched = GameObject.Find("Point-" + pointId);
        ExplodePoint(objectTouched);
        _currentOtherScore += sumScore;
        SetOtherScore();
    }

    private void OnApplicationQuit()
    {
        if (Client != null)
            Client.Disconnect();
    }

    private void SetTimer()
    {
        TxtTimer.text = _timerCount.ToString("0.00");
    }

    private void SetSelfScore()
    {
        if (IsFirstPlayer)
            TxtScoreP1.text = _currentSelfScore.ToString();
        else
            TxtScoreP2.text = _currentSelfScore.ToString();
    }

    private void SetOtherScore()
    {
        if (IsFirstPlayer)
            TxtScoreP2.text = _currentOtherScore.ToString();
        else
            TxtScoreP1.text = _currentOtherScore.ToString();
    }
    #endregion

    public class GamePoint
    {
        public short Id { get; set; }
        public GamePointType Type { get; set; }
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
