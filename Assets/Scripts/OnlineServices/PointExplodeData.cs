using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.OnlineServices
{
    [Serializable]
    public class PointExplodeData
    {
        public short PointId { get; set; }
        public byte SumScore { get; set; }
    }
}
