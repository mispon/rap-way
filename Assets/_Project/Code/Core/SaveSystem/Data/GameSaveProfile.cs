using System;
using System.Collections.Generic;
using System.Globalization;

namespace RapWay.Core.SaveSystem.Data
{
    [Serializable]
    public class GameSaveProfile
    {
        public string GameVersion = "3.0.0";
        public string LastSaveTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        public Dictionary<string, object> State = new();
    }
}