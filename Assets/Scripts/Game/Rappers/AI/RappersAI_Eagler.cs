using Core.Analytics;
using Enums;
using Game.Rappers.Desc;
using Game.Settings;
using Game.SocialNetworks.Eagler;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using MessageBroker.Messages.UI;
using UI.Enums;
using UI.Windows.GameScreen.SocialNetworks;
using UnityEngine;
using RappersAPI = Game.Rappers.RappersPackage;
using PlayerAPI = Game.Player.PlayerPackage;

namespace Game.Rappers.AI
{
    public partial class RappersAI
    {
        private static void DoEagler(RapperInfo rapper, GameSettings settings)
        {
            rapper.Cooldown = settings.Rappers.EaglerCooldown;

            var isPlayerTarget = CanInteractPlayer(rapper.Fans, PlayerAPI.Data.Fans) && RollDice() <= settings.Rappers.PlayerEagleChance;
            var target = isPlayerTarget
                ? PlayerAPI.Data.Info.NickName
                : GetRandomRapperName(rapper.Name);

            Debug.Log($"[RAPPER AI] {rapper.Name} post eagle about {target}");
            AnalyticsManager.LogEvent(FirebaseGameEvents.RapperAI_PostEagle);

            if (isPlayerTarget)
            {
                MsgBroker.Instance.Publish(new EmailMessage
                {
                    Title       = "rapper_eagle_player_title",
                    TitleArgs   = new[] {rapper.Name},
                    Content     = "rapper_eagle_player_text",
                    ContentArgs = new[] {rapper.Name},
                    Sender      = "personal.assistant@mail.com",
                    Sprite      = rapper.Avatar,
                    mainAction = () =>
                    {
                        MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.GameScreen));
                        MsgBroker.Instance.Publish(new WindowControlMessage
                        {
                            Type    = WindowType.SocialNetworks,
                            Context = SocialNetworksTabType.Eagler
                        });
                    }
                });
            }

            EaglerManager.Instance.CreateRapperEagle(rapper, target);
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