using Assets.Scripts;
using System;
namespace Assets.Scripts
{
    [Serializable]
    public class GamePoint
    {
        [NonSerialized]
        private bool _instatiated;
        [NonSerialized]
        private float _timeExplode;
        [NonSerialized]
        private bool? _isExplodeBySelf;

        public short Id { get; set; }
        public short Type { get; set; }
        public float Time { get; set; }
        public bool Instatiated
        {
            get
            {
                return _instatiated;
            }

            set
            {
                _instatiated = value;
            }
        }
        public bool? IsExplodeBySelf
        {
            get
            {
                return _isExplodeBySelf;
            }

            set
            {
                _isExplodeBySelf = value;
            }
        }
        public float TimeExplode
        {
            get
            {
                return _timeExplode;
            }

            set
            {
                _timeExplode = value;
            }
        }
        public byte ValuePoint
        {
            get
            {
                return Type == (short)GamePointType.NormalPoint ? (byte)1 : (byte)5;
            }
        }
        public byte RealValuePoint { get; set; }
        public enum GamePointType
        {
            NormalPoint,
            SpecialPoint,
            PointExplode,
            PointExplodeSpread
        }
    }
}
