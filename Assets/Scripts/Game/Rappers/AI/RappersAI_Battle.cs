using System.Collections.Generic;
using Enums;
using Game.Rappers.Desc;
using Game.Settings;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using MessageBroker.Messages.UI;
using UI.Enums;
using PlayerAPI = Game.Player.PlayerPackage;

namespace Game.Rappers.AI
{
    public partial class RappersAI
    {
        private static void ProposeBattle(RapperInfo rapper, GameSettings settings)
        {
            if (!CanInteractPlayer(rapper.Fans, PlayerAPI.Data.Fans))
            {
                return;
            }

            rapper.Cooldown = settings.Rappers.BattleCooldown;

            MsgBroker.Instance.Publish(new EmailMessage
            {
                Title       = "rapper_offers_battle_title",
                TitleArgs   = new[] {rapper.Name},
                Content     = "rapper_offers_battle_text",
                ContentArgs = new[] {PlayerAPI.Data.Info.NickName, rapper.Name},
                Sender      = "personal.assistant@mail.com",
                Sprite      = rapper.Avatar,
                mainAction = () =>
                {
                    MsgBroker.Instance.Publish(new WindowControlMessage
                    {
                        Type = WindowType.RapperConversationsResult,
                        Context = new Dictionary<string, object>
                        {
                            ["rapper"]        = rapper,
                            ["conv_type"]     = ConversationType.Battle,
                            ["rapper_points"] = 0,
                            ["player_points"] = 100
                        }
                    });
                }
            });
        }
    }
}