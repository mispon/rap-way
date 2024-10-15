using System.Collections.Generic;
using Core.Analytics;
using Enums;
using Game.Rappers.Desc;
using Game.Settings;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using MessageBroker.Messages.UI;
using UI.Enums;
using UnityEngine;
using PlayerAPI = Game.Player.PlayerPackage;

namespace Game.Rappers.AI
{
    public partial class RappersAI
    {
        private static void ProposeFeat(RapperInfo rapper, GameSettings settings)
        {
            if (!CanInteractPlayer(rapper.Fans, PlayerAPI.Data.Fans))
            {
                return;
            }

            Debug.Log($"[RAPPER AI] {rapper.Name} offers feat to player");
            AnalyticsManager.LogEvent(FirebaseGameEvents.RapperAI_FeatPlayer);

            rapper.Cooldown = settings.Rappers.FeatCooldown;

            MsgBroker.Instance.Publish(new EmailMessage
            {
                Title       = "rapper_offers_feat_title",
                TitleArgs   = new[] {rapper.Name},
                Content     = "rapper_offers_feat_text",
                ContentArgs = new[] {PlayerAPI.Data.Info.NickName, rapper.Name},
                Sender      = "personal.assistant@mail.com",
                Sprite      = rapper.Avatar,
                Type        = EmailsType.FeatOffer,
                mainAction = () =>
                {
                    MsgBroker.Instance.Publish(new WindowControlMessage
                    {
                        Type = WindowType.RapperConversationsResult,
                        Context = new Dictionary<string, object>
                        {
                            ["rapper"]        = rapper,
                            ["conv_type"]     = ConversationType.Feat,
                            ["rapper_points"] = 0,
                            ["player_points"] = 100
                        }
                    });
                }
            });
        }
    }
}