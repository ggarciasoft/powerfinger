using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.OnlineServices
{
    public interface IOnlineService : IDisposable
    {
        void SetOnRoomFullEvent(Action action);
        void SetOnPointExplodeEvent(Action<PointExplodeData> action);

        void Initialize();
        OnlineStatus GetConnectionStatus();
        bool CreateRoom(string roomId);
        bool JoinRoom(string roomId);
        bool LeaveRoom();
        bool SendPointExplode(PointExplodeData data);
        bool Sync();
    }
}
