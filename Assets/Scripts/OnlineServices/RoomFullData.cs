﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.OnlineServices
{
    public class RoomFullData
    {
        public bool LocalUserIsFirstPlayer { get; set; }
        public string FirstPlayerNickName { get; set; }
        public string SecondPlayerNickName { get; set; }
    }
}
