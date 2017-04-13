using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Assets.Scripts;
using UnityEngine.SceneManagement;
using Assets.Scripts.OnlineServices;
using UnityEngine.Events;
//TODO: agregar logica en el resume, cuando el juego este corriendo
public class PlayboardOnlineVersusManager : CommonManager
{
    #region Properties
    public List<GameObject> lstPrefabPoints;

    private Dictionary<GamePoint.GamePointType, GameObject> _listGamePointsType;
    private List<GamePoint> _listGamePointsGenerated;
    private GameObject _playboard;
    private Text _txtTimer, _txtScoreP1, _txtScoreP2, _txtNameP1, _txtNameP2, _lblWaitingForPlayer;
    private float _timerCount = 30f;
    private bool _gameInitiate, _tryLeavingRoom, _waitingStart;
    private short _currentSelfScore, _currentOtherScore;

    private bool _isFirstPlayer;

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
        _waitingStart = true;

        OnlineService.SetOnRoomFullEvent(OnRoomFull);
        OnlineService.SetOnPointExplodeEvent(PointExplodeFromOponent);
        OnlineService.SetOnOponentLeaveRoomEvent(OponentLeaveRoom);

        Initialize();

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

    private void OnRoomFull(RoomFullData data)
    {
        _gameInitiate = true;
        _waitingStart = false;
        _isFirstPlayer = data.LocalUserIsFirstPlayer;
        TxtNameP1.text = data.FirstPlayerNickName;
        TxtNameP2.text = data.SecondPlayerNickName;
        InvokeRepeating("HideBlockImage", 0, 0.01f);
    }

    private void Initialize()
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
            if (_waitingStart)
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

        var particle = _temp.GetComponent<ParticleSystem>().main;
        particle.simulationSpeed = 2f;
        DestroyImmediate(pointToExplode);

        Destroy(_temp, 1.5f);
    }

    private void UpdateGame()
    {
        if (_timerCount <= 0)
        {
            GameFinish();
            return;
        }

        var lstPointToInstatiate = _listGamePointsGenerated.Where(o => o.Time >= _timerCount && !o.Instatiated);

        if (lstPointToInstatiate.Any())
        {
            foreach (var item in lstPointToInstatiate)
            {
                var point = _listGamePointsType[item.Type];
                var particle = point.GetComponent<ParticleSystem>().main;

                float x = Random.Range(-6.72f, 6.72f), y = Random.Range(-2.9f, 5.82f), z = 0;

                item.Instatiated = true;

                if (item.Type == GamePoint.GamePointType.NormalPoint)
                    particle.simulationSpeed = 0.4f;

                point = Instantiate(point, new Vector3(x, y, z), Quaternion.identity) as GameObject;
                point.name = "Point-" + item.Id;
                Destroy(point, particle.duration + 1);
            }
        }

        _timerCount -= 0.01f;
        SetTimer();
    }

    private void GameFinish()
    {
        CancelInvoke("UpdateGame");
        _gameInitiate = false;
        MessageBox("GAME FINISH!!!", onClose: () => SceneManager.LoadScene("GameFinish"));
    }

    private void LeaveRoom()
    {
        OnlineService.LeaveRoom();
        BackFromGame = true;
        SceneManager.LoadScene("GameFinish");
    }

    private void OponentLeaveRoom()
    {
        CancelInvoke("UpdateGame");
        _gameInitiate = false;
        MessageBox("Your oponent leave the room, you win!", onClose: () => SceneManager.LoadScene("GameFinish"));
    }

    private void Update()
    {
        if (_waitingStart && Input.touchCount > 0 && Input.GetTouch(0).tapCount == 2)
        {
            OnlineService.LeaveRoom();
            BackFromGame = true;
            SceneManager.LoadScene("MainMenu");
        }

        if (UpdateServerService)
            OnlineService.Sync();

        if (!_gameInitiate || _tryLeavingRoom) return;

        if (Input.GetKeyDown(KeyCode.Escape) && !_tryLeavingRoom)
        {
            _tryLeavingRoom = true;
            MessageBox("If you leave the game, you will lose, are you sure?", LeaveRoom, () => _tryLeavingRoom = false);
        }

        GameObject objectTouched = null;
        var isTouched = Input.GetMouseButtonDown(0) || Input.touchCount > 0;

        if (!isTouched) return;

        if (Input.GetMouseButtonDown(0))
            OnObjectTouched(objectTouched, Input.mousePosition);
        else
            for (int i = 0; i < Input.touchCount; i++)
            {
                var touch = Input.GetTouch(i);
                if (touch.phase != TouchPhase.Began) continue;
                OnObjectTouched(objectTouched, touch.position);
            }

        SetSelfScore();
    }

    private void OnObjectTouched(GameObject objectTouched, Vector3 position)
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

        OnlineService.SendPointExplode(new PointExplodeData
        {
            PointId = id,
            SumScore = sumScore
        });

        ExplodePoint(objectTouched);
    }

    private void PointExplodeFromOponent(PointExplodeData pointExplode)
    {
        //var point = _listGamePointsGenerated.Single(o => o.Id == pointId);
        var objectTouched = GameObject.Find("Point-" + pointExplode.PointId);
        ExplodePoint(objectTouched);
        _currentOtherScore += pointExplode.SumScore;
        SetOtherScore();
    }

    private void OnApplicationQuit()
    {
        if (OnlineService != null)
            OnlineService.Dispose();
    }

    private void SetTimer()
    {
        TxtTimer.text = _timerCount.ToString("0.00");
    }

    private void SetSelfScore()
    {
        if (_isFirstPlayer)
            TxtScoreP1.text = _currentSelfScore.ToString();
        else
            TxtScoreP2.text = _currentSelfScore.ToString();
    }

    private void SetOtherScore()
    {
        if (_isFirstPlayer)
            TxtScoreP2.text = _currentOtherScore.ToString();
        else
            TxtScoreP1.text = _currentOtherScore.ToString();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if(hasFocus && _gameInitiate)
        {
            CancelInvoke("UpdateGame");
            _gameInitiate = false;
            MessageBox("You Leave the room!", onClose: () => SceneManager.LoadScene("GameFinish"));
        }
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
