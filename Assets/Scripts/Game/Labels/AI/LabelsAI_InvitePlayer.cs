using Enums;
using Game.Labels.Desc;
using Game.Settings;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using MessageBroker.Messages.UI;
using UI.Enums;
using UnityEngine;
using PlayerAPI = Game.Player.PlayerPackage;

namespace Game.Labels.AI
{
    public partial class LabelsAI
    {
        private static void DoInvitePlayer(LabelInfo label, GameSettings settings)
        {
            const int minFansCount = 50_000;

            if (PlayerAPI.Data.Label != "")
            {
                // already in label
                return;
            }

            if (PlayerAPI.Data.Fans < minFansCount)
            {
                // too low count of fans
                return;
            }

            Debug.Log($"[LABEL AI] {label.Name} send invite to player");
            label.Cooldown = settings.Labels.InvitePlayerCooldown;

            MsgBroker.Instance.Publish(new EmailMessage
            {
                Type      = EmailsType.LabelsContract,
                Title     = "label_contract_greeting",
                TitleArgs = new[] {label.Name},
                Content   = "label_contract_text",
                ContentArgs = new[]
                {
                    PlayerAPI.Data.Info.NickName,
                    label.Name,
                    label.Name
                },
                Sender = $"{label.Name.ToLower()}@label.com",
                Sprite = label.Logo,
                mainAction = () =>
                {
                    MsgBroker.Instance.Publish(new WindowControlMessage
                    {
                        Type    = WindowType.LabelContract,
                        Context = label
                    });
                }
            });
        }
    }
}