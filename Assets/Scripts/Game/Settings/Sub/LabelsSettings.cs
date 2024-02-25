using System;
using UnityEngine;

namespace Game.Settings.Sub
{
    [Serializable]
    public class LabelsSettings
    {
        public int MinLevel = 0;
        public int MaxLevel = 5;

        [Space]
        public int[] ExpToLevelUp = {100, 200, 300, 400, 500, 0};
        public int   ExpChangeValue = 100;
        
        [Space]
        public int InvitePlayerFrequency = 3;
        public int InviteRapperFrequency = 1;
        
        [Space]
        // todo: remove it, decision about leave label or not should be in rappers AI
        public int RappersActionsFrequency = 1;
    }
}