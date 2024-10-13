using System;
using UnityEngine;

namespace Game.Settings.Sub
{
    [Serializable]
    public class LabelsSettings
    {
        public int   MinLevel       = 0;
        public int   MaxLevel       = 5;
        public int   Fee            = 20;
        public int[] ExpToLevelUp   = {100, 200, 300, 400, 500, 0};
        public int   ExpChangeValue = 100;

        [Header("AI Cooldowns")]
        public int InvitePlayerCooldown = 90;
        public int InviteRapperCooldown = 60;
    }
}