using Assets.Scripts;
using System;
namespace Assets.Scripts
{
    [Serializable]
    public class GamePoint
    {
        public short Id { get; set; }
        public short Type { get; set; }
        public float Time { get; set; }
        public float TimeExplode { get; set; }
        public bool? IsExplodeBySelf { get; set; }
        public bool Instatiated { get; set; }

        public enum GamePointType
        {
            NormalPoint,
            SpecialPoint,
            PointExplode,
            PointExplodeSpread
        }
    }
}
