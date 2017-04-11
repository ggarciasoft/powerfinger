using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.OnlineServices
{
    public interface IOnlineService : IDisposable
    {
        void SetOnRoomFullEvent(Action<RoomFullData> action);
        void SetOnPointExplodeEvent(Action<PointExplodeData> action);
        void SetOnOponentLeaveRoomEvent(Action action);

        void Initialize();
        OnlineStatus GetConnectionStatus();
        bool CreateRoom(string roomId = null);
        bool JoinRoom(string roomId = null);
        bool LeaveRoom();
        bool SendPointExplode(PointExplodeData data);
        bool Sync();
    }
}
