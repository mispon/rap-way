using Game.Rappers.Desc;
using Game.Settings;
using UnityEngine;

namespace Game.Rappers.AI
{
    public partial class RappersAI
    {
        private static void DoEagler(RapperInfo rapperInfo, GameSettings settings)
        {
            rapperInfo.Cooldown = settings.Rappers.EaglerCooldown;
            Debug.Log($"{rapperInfo.Name} write eagle");
            // TODO
        }
    }
}