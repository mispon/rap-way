using System.Collections.Generic;
using Core.Analytics;
using Enums;
using Game.Rappers.Desc;
using Game.Settings;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using UI.Enums;
using UnityEngine;
using PlayerAPI = Game.Player.PlayerPackage;

namespace Game.Rappers.AI
{
    public partial class RappersAI
    {
        private static void ProposeBattle(RapperInfo rapper, GameSettings settings, ImagesBank imagesBank)
        {
            rapper.Cooldown = settings.Rappers.BattleCooldown;

            if (CanInteractPlayer(rapper.Fans, PlayerAPI.Data.Fans) && RollDice() <= settings.Rappers.PlayerBattleChance)
            {
                Debug.Log($"[RAPPER AI] {rapper.Name} offer battle to player");
                AnalyticsManager.LogEvent(FirebaseGameEvents.RapperAI_BattlePlayer);

                MsgBroker.Instance.Publish(new EmailMessage
                {
                    Title       = "rapper_offers_battle_title",
                    TitleArgs   = new[] {rapper.Name},
                    Content     = "rapper_offers_battle_text",
                    ContentArgs = new[] {PlayerAPI.Data.Info.NickName, rapper.Name},
                    Sender      = "personal.assistant@mail.com",
                    Sprite      = rapper.Avatar,
                    Type        = EmailsType.BattleOffer,
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
            } else
            {
                var opponent = GetRandomRapperName(rapper.Name);

                Debug.Log($"[RAPPER AI] {rapper.Name} do battle with {opponent}");
                AnalyticsManager.LogEvent(FirebaseGameEvents.RapperAI_BattleRapper);

                MsgBroker.Instance.Publish(new NewsMessage
                {
                    Text = "news_battle_finished",
                    TextArgs = RollDice() > 50
                        ? new[] {opponent, rapper.Name}
                        : new[] {rapper.Name, opponent},
                    Sprite     = imagesBank.NewsBattle,
                    Popularity = rapper.Fans
                });
            }
        }
    }
}