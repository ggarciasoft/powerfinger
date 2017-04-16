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
        void SetOnOponentDeclinedEvent(Action action);
        void SetOnShowWaitingRoomEvent(Action action);
        void SetMessageEvent(Action<string, bool> action);
        void SetOnStartGameEvent(Action action);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="func">Function To Generate List Game Point and return the list</param>
        void SetGeneratePointFunc(Func<GamePoint[]> func);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">Action to set the List Game Point</param>
        void SetGeneratedPointAction(Action<GamePoint[]> action);

        void Initialize();
        OnlineStatus GetConnectionStatus();
        bool CreateRoom(string roomId = null);
        bool InviteRoom();
        bool JoinRoom(string roomId = null);
        bool LeaveRoom();
        bool SendPointExplode(PointExplodeData data);
        bool Sync();
        bool IsRoomConected();
    }
}
