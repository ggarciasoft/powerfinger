using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.OnlineServices
{
    public class MainOnlineService
    {
        public static IOnlineService OnlineService
        {
            get
            {
                return (IOnlineService)new object();// PowerFingerBalancingClient.Instance;
            }
        }
    }

    public enum OnlineStatus
    {
        Uninitialized,
        Connecting,
        Connected,
        Disconnecting,
        Disconnected,
        Unknown
    }

    public enum EventDataCode
    {
        PointExplode
    }

    public enum EventDataParameter
    {
        PointId,
        SumScore
    }
}
