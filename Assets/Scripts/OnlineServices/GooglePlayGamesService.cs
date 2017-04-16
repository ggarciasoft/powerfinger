using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using PlayFab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.OnlineServices
{
    public class GooglePlayGamesService : IOnlineService
    {
        #region properties
        private RealTimeMultiplayerListenerImplementation _listenerInstance = new RealTimeMultiplayerListenerImplementation();
        private static GooglePlayGamesService _instance;
        private IRealTimeMultiplayerClient RealTime
        {
            get
            {
                return PlayGamesPlatform.Instance.RealTime;
            }
        }

        public static GooglePlayGamesService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GooglePlayGamesService();
                }
                return _instance;
            }
        }

        #endregion

        public bool CreateRoom(string roomId = null)
        {
            RealTime.CreateQuickGame(1, 1, 0, _listenerInstance);
            return true;
        }

        public void Dispose()
        {
        }

        public OnlineStatus GetConnectionStatus()
        {
            return PlayGamesPlatform.Instance.localUser.authenticated ? OnlineStatus.Connected : OnlineStatus.Disconnected;
        }

        public void Initialize()
        {
        }

        public bool JoinRoom(string roomId = null)
        {
            RealTime.AcceptFromInbox(_listenerInstance);
            return true;
        }

        public bool InviteRoom()
        {
            PlayGamesPlatform.Instance.RealTime.CreateWithInvitationScreen(1, 1, 0, _listenerInstance);
            return true;
        }

        public bool LeaveRoom()
        {
            RealTime.LeaveRoom();
            return true;
        }

        public bool SendPointExplode(PointExplodeData data)
        {
            //byte + byte + short length
            var arr = new List<byte>(4);
            arr.Add((byte)MessageType.PointExplode);
            arr.Add(data.SumScore);
            arr.AddRange(BitConverter.GetBytes(data.PointId));
            RealTime.SendMessageToAll(true, arr.ToArray());
            return true;
        }

        public void SetOnPointExplodeEvent(Action<PointExplodeData> action)
        {
            _listenerInstance.OnPointExplodeAction = action;
        }

        public void SetOnRoomFullEvent(Action<RoomFullData> action)
        {
            _listenerInstance.OnRoomFullAction = action;
        }

        public bool Sync()
        {
            return true;
        }

        public void SetOnOponentLeaveRoomEvent(Action action)
        {
            _listenerInstance.OnOponentLeftAction = action;
        }

        public bool IsRoomConected()
        {
            return RealTime.IsRoomConnected();
        }

        public void SetOnOponentDeclinedEvent(Action action)
        {
            _listenerInstance.OnOponentDeclinedAction = action;
        }

        public void SetOnShowWaitingRoomEvent(Action action)
        {
            _listenerInstance.OnShowWaitingRoomAction = action;
        }

        public void SetMessageEvent(Action<string, bool> action)
        {
            _listenerInstance.MessageAction = action;
        }

        public void SetOnStartGameEvent(Action action)
        {
            _listenerInstance.OnStartGameAction = action;
        }

        public void SetGeneratePointFunc(Func<GamePoint[]> func)
        {
            _listenerInstance.GenerateGamePointFunc = func;
        }

        public void SetGeneratedPointAction(Action<GamePoint[]> action)
        {
            _listenerInstance.SetGeneratedPointAction = action;
        }

        private class RealTimeMultiplayerListenerImplementation : RealTimeMultiplayerListener
        {
            public Action<RoomFullData> OnRoomFullAction { get; set; }
            public Action<PointExplodeData> OnPointExplodeAction { get; set; }
            public Action OnOponentLeftAction { get; set; }
            public Action OnOponentDeclinedAction { get; set; }
            public Action OnShowWaitingRoomAction { get; set; }
            public Action OnStartGameAction { get; set; }
            public Action<string, bool> MessageAction { get; set; }
            public Func<GamePoint[]> GenerateGamePointFunc { get; set; }
            public Action<GamePoint[]> SetGeneratedPointAction { get; set; }
            public bool IsInWaitingRoom { get; set; }

            public RealTimeMultiplayerListenerImplementation()
            {

            }

            public void OnLeftRoom()
            {
            }

            public void OnParticipantLeft(Participant participant)
            {
                if (IsInWaitingRoom && OnOponentDeclinedAction != null)
                    OnOponentDeclinedAction();
            }

            public void OnPeersConnected(string[] participantIds)
            {
            }

            public void OnPeersDisconnected(string[] participantIds)
            {
                if (OnOponentLeftAction != null)
                    OnOponentLeftAction();
            }

            public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
            {
                if (data[0] == (byte)MessageType.GeneratePoint)
                {
                    var bf = new BinaryFormatter();
                    using (var stream = new MemoryStream(data, 1, data.Length - 1))
                        SetGeneratedPointAction((GamePoint[])bf.Deserialize(stream));
                    data = new byte[1];
                    data[0] = (byte)MessageType.StartGame;
                    PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, data);
                    OnStartGameAction();
                }
                else if (data[0] == (byte)MessageType.StartGame)
                {
                    OnStartGameAction();
                }
                else if (data[0] == (byte)MessageType.PointExplode)
                {
                    if (OnPointExplodeAction != null)
                        OnPointExplodeAction(new PointExplodeData
                        {
                            SumScore = data[1],
                            PointId = BitConverter.ToInt16(data, 2)
                        });
                }
            }

            private void SendGeneratedPoint()
            {
                var lstPoints = GenerateGamePointFunc();
                var bf = new BinaryFormatter();
                using (var stream = new MemoryStream())
                {
                    bf.Serialize(stream, lstPoints);
                    var obj = stream.ToArray();
                    var lst = new List<byte>(obj.Length + 1);
                    lst.Add((byte)MessageType.GeneratePoint);
                    lst.AddRange(obj);
                    PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, lst.ToArray());
                }
            }

            public void OnRoomConnected(bool success)
            {
                var participants = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
                if (participants.Count == 2)
                {
                    if (OnRoomFullAction != null)
                        OnRoomFullAction(new RoomFullData
                        {
                            FirstPlayerNickName = participants[1].DisplayName,
                            SecondPlayerNickName = participants[0].DisplayName,
                            LocalUserIsFirstPlayer = PlayGamesPlatform.Instance.localUser.userName == participants[1].DisplayName
                        });
                    IsInWaitingRoom = false;

                    if (PlayGamesPlatform.Instance.localUser.userName == participants[1].DisplayName)
                        SendGeneratedPoint();

                }
            }

            public void OnRoomSetupProgress(float percent)
            {
                if (!IsInWaitingRoom && OnShowWaitingRoomAction != null)
                {
                    OnShowWaitingRoomAction();
                    IsInWaitingRoom = true;
                }
            }
        }

        public enum MessageType
        {
            PointExplode,
            GeneratePoint,
            StartGame
        }
    }
}
