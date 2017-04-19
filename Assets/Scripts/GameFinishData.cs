using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class GameFinishData
    {
        public GamePoint[] ListGamePoints { get; set; }
        public bool? IsWinner { get; set; }

        public GameFinishData()
        {
            ListGamePoints = new GamePoint[0];
        }
    }
}
