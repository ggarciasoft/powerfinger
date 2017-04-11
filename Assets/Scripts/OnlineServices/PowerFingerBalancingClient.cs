using ExitGames.Client.Photon.LoadBalancing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using Assets.Scripts.OnlineServices;

namespace Assets.Scripts.OnlineServices
{
    public class PowerFingerBalancingClient : LoadBalancingClient, IOnlineService
    {
        private Action<RoomFullData> _onRoomFullAction;
        private Action<PointExplodeData> _onPointExplodeAction;

        private static PowerFingerBalancingClient _client;

        public static PowerFingerBalancingClient Instance
        {
            get
            {
                if (_client == null)
                    _client = new PowerFingerBalancingClient()
                    {
                        AppId = "bca9b338-6e2a-4c80-b665-e884285f1f78"
                    };

                return _client;
            }
        }

        public void Initialize()
        {
            ConnectToRegionMaster(Preferences.RegionName);
        }

        public OnlineStatus GetConnectionStatus()
        {
            switch (State)
            {
                case ClientState.Uninitialized:
                    return OnlineStatus.Uninitialized;
                case ClientState.ConnectingToGameserver:
                case ClientState.ConnectedToGameserver:
                case ClientState.Joining:
                case ClientState.Joined:
                case ClientState.ConnectingToMasterserver:
                case ClientState.ConnectedToMaster:
                case ClientState.ConnectedToNameServer:
                case ClientState.ConnectingToNameServer:
                    return OnlineStatus.Connecting;
                case ClientState.JoinedLobby:
                    return OnlineStatus.Connected;
                case ClientState.DisconnectingFromMasterserver:
                case ClientState.Leaving:
                case ClientState.DisconnectingFromGameserver:
                case ClientState.Disconnecting:
                case ClientState.DisconnectingFromNameServer:
                    return OnlineStatus.Disconnecting;
                case ClientState.Disconnected:
                    return OnlineStatus.Disconnected;
                default:
                    return OnlineStatus.Unknown;
            }
        }

        public override void OnEvent(EventData photonEvent)
        {
            base.OnEvent(photonEvent);

            Hashtable values;

            switch (photonEvent.Code)
            {
                case EventCode.Join:
                    if (_onRoomFullAction != null && CurrentRoom.PlayerCount == 2)
                        _onRoomFullAction(new RoomFullData
                        {
                            FirstPlayerNickName = CurrentRoom.Players.First().Value.NickName,
                            SecondPlayerNickName = CurrentRoom.Players.Last().Value.NickName
                        });
                    break;
                case (byte)EventDataCode.PointExplode:
                    if (_onPointExplodeAction != null)
                    {
                        values = photonEvent.Parameters[ParameterCode.CustomEventContent] as Hashtable;
                        _onPointExplodeAction(new PointExplodeData
                        {
                            PointId = (short)values[(byte)EventDataParameter.PointId],
                            SumScore = (byte)values[(byte)EventDataParameter.SumScore]
                        });
                    }
                    break;
                default:
                    break;
            }
        }

        public bool CreateRoom(string roomId = null)
        {
            return OpCreateRoom(roomId, new RoomOptions
            {
                MaxPlayers = 2,
                EmptyRoomTtl = 2000,
                PlayerTtl = 2000,
                CheckUserOnJoin = true
            }, TypedLobby.Default);
        }

        public bool JoinRoom(string roomId = null)
        {
            return String.IsNullOrEmpty(roomId) ? OpJoinRandomRoom(null, 0) : OpJoinRoom(roomId);
        }

        public bool LeaveRoom()
        {
            return OpLeaveRoom();
        }

        public bool Sync()
        {
            Service();
            return true;
        }

        public void Dispose()
        {
            Disconnect();
        }

        public bool SendPointExplode(PointExplodeData data)
        {
            Hashtable evData = new Hashtable();
            evData[(byte)EventDataParameter.PointId] = data.PointId;
            evData[(byte)EventDataParameter.SumScore] = data.SumScore;

            return OpRaiseEvent((byte)EventDataCode.PointExplode, evData, true, RaiseEventOptions.Default);
        }

        public void SetOnRoomFullEvent(Action<RoomFullData> action)
        {
            _onRoomFullAction = action;
        }

        public void SetOnPointExplodeEvent(Action<PointExplodeData> action)
        {
            _onPointExplodeAction = action;
        }

        public void SetOnOponentLeaveRoomEvent(Action action)
        {
            throw new NotImplementedException();
        }
    }
}
