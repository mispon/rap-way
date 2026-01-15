using System;
using System.Collections.Generic;
using System.Globalization;

namespace RapWay.Core.SaveSystem.Data
{
    [Serializable]
    public class GameSaveProfile
    {
        public string LastSaveTime;
        public string GameVersion;
        
        public Dictionary<string, object> State = new();

        public GameSaveProfile()
        {
            LastSaveTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            GameVersion = "3.0.0";
        }
    }
}