using System;

namespace Game.Settings.Sub
{
    [Serializable]
    public class RappersSettings
    {
        public int MinFans = 1;
        public int MaxFans = 500_000_000;
        public int FansUpdateFrequency = 2;
        public int ConversationDuration = 30;
    }
}