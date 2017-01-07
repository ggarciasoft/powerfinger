using ExitGames.Client.Photon.LoadBalancing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;

namespace Assets.Scripts
{
    public class PowerFingerBalancingClient : LoadBalancingClient
    {
        public Action OnRoomFullAction;
        public Action<PointExplodeData> OnPointExplodeAction;

        private static PowerFingerBalancingClient _client;

        public static PowerFingerBalancingClient Instance
        {
            get
            {
                if (_client == null)
                {
                    _client = new PowerFingerBalancingClient();
                    _client.AppId = "bca9b338-6e2a-4c80-b665-e884285f1f78";
                }

                return _client;
            }
        }


        public override void OnEvent(EventData photonEvent)
        {
            base.OnEvent(photonEvent);

            Hashtable values;

            switch (photonEvent.Code)
            {
                case EventCode.Join:
                    if (OnRoomFullAction != null && CurrentRoom.PlayerCount == 2)
                        OnRoomFullAction();
                    break;
                case (byte)CommonManager.EventDataCode.PointExplode:
                    if (OnPointExplodeAction != null)
                    {
                        values = photonEvent.Parameters[ParameterCode.CustomEventContent] as Hashtable;
                        OnPointExplodeAction(new PointExplodeData
                        {
                            PointId = (short)values[(byte)CommonManager.EventDataParameter.PointId],
                            SumScore = (byte)values[(byte)CommonManager.EventDataParameter.SumScore]
                        });
                    } 
                    break;
                default:
                    break;
            }
        }
    }
}
