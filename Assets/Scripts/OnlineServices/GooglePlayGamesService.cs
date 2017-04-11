using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

        public bool LeaveRoom()
        {
            RealTime.LeaveRoom();
            return true;
        }

        public bool SendPointExplode(PointExplodeData data)
        {
            //byte + short length
            var arr = new List<byte>(3);
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

        private class RealTimeMultiplayerListenerImplementation : RealTimeMultiplayerListener
        {
            public Action<RoomFullData> OnRoomFullAction { get; set; }
            public Action<PointExplodeData> OnPointExplodeAction { get; set; }
            public Action OnOponentLeftAction { get; set; }

            public RealTimeMultiplayerListenerImplementation()
            {

            }

            public void OnLeftRoom()
            {
            }

            public void OnParticipantLeft(Participant participant)
            {
            }

            public void OnPeersConnected(string[] participantIds)
            {
            }

            public void OnPeersDisconnected(string[] participantIds)
            {
                OnOponentLeftAction();
            }

            public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
            {
                OnPointExplodeAction(new PointExplodeData
                {
                    SumScore = data[0],
                    PointId = BitConverter.ToInt16(data, 1)
                });
            }

            public void OnRoomConnected(bool success)
            {
                var participants = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
                if (participants.Count == 2)
                    OnRoomFullAction(new RoomFullData
                    {
                        FirstPlayerNickName = participants[1].DisplayName,
                        SecondPlayerNickName = participants[0].DisplayName
                    });
            }

            public void OnRoomSetupProgress(float percent)
            {
            }
        }
    }
}
