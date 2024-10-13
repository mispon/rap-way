using Game.Rappers.Desc;
using Game.Settings;
using Game.SocialNetworks.Eagler;
using RappersAPI = Game.Rappers.RappersPackage;
using PlayerAPI = Game.Player.PlayerPackage;

namespace Game.Rappers.AI
{
    public partial class RappersAI
    {
        private static void DoEagler(RapperInfo rapper, GameSettings settings)
        {
            rapper.Cooldown = settings.Rappers.EaglerCooldown;

            const int playerTargetChance = 25;
            var       isPlayerTarget     = CanInteractPlayer(rapper.Fans, PlayerAPI.Data.Fans) && RollDice() <= playerTargetChance;
            
            var target = isPlayerTarget
                ? PlayerAPI.Data.Info.NickName
                : GetRandomRapperName(rapper.Name);

            if (isPlayerTarget)
            {
                // send email
            }

            EaglerManager.Instance.CreateRapperEagle(rapper, target);
        }

        private static bool CanInteractPlayer(int rapperFans, int playerFans)
        {
            if (playerFans >= rapperFans)
            {
                return true;
            }

            const int maxDiff = 30;

            var diff = playerFans / rapperFans * 100;
            return diff < maxDiff;
        }

        private static string GetRandomRapperName(string self)
        {
            while (true)
            {
                var rapperName = RappersAPI.Instance.GetRandom().Name;
                if (rapperName != self)
                {
                    return rapperName;
                }
            }
        }
    }
}