using System;
using UnityEngine;

namespace Game.Settings.Sub
{
    [Serializable]
    public class TeamSettings
    {
        [Range(1, 100), Tooltip("Manager cooldown, days")]
        public int ManagerCooldown = 60;
        [Range(1, 50), Tooltip("Pr-Manager cooldown, days")]
        public int PrManagerCooldown = 30;
    }
}