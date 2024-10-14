using Game.Rappers.Desc;
using Game.Settings;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using UnityEngine;
using RappersAPI = Game.Rappers.RappersPackage;
using LabelsAPI = Game.Labels.LabelsPackage;

namespace Game.Rappers.AI
{
    public partial class RappersAI
    {
        public void TryJoinLabel(int rapperId, string labelName, GameSettings settings)
        {
            var rapper = RappersAPI.Instance.Get(rapperId);
            var label  = LabelsAPI.Instance.Get(labelName);

            if (rapper.Label != "")
            {
                return;
            }

            rapper.Cooldown = settings.Rappers.LabelCooldown;

            var rapperPrestige = RappersAPI.GetRapperPrestige(rapper);
            var labelPrestige  = LabelsAPI.Instance.GetPrestige(label);
            var chance         = CalcChance(labelPrestige, rapperPrestige);

            if (RollDice() >= chance)
            {
                // rapper decide don't join the label
                return;
            }

            Debug.Log($"[RAPPER AI] {rapper.Name} join label {label.Name}");
            rapper.Label = label.Name;

            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text       = "news_rapper_join_label",
                TextArgs   = new[] {rapper.Name, label.Name},
                Sprite     = label.Logo,
                Popularity = rapper.Fans
            });
        }

        private static void TryLeaveLabel(RapperInfo rapper, GameSettings settings)
        {
            var label = LabelsAPI.Instance.Get(rapper.Label);
            if (label == null)
            {
                return;
            }

            rapper.Cooldown = settings.Rappers.LabelCooldown;

            var rapperPrestige = RappersAPI.GetRapperPrestige(rapper);
            var labelPrestige  = LabelsAPI.Instance.GetPrestige(label);
            var chance         = CalcChance(rapperPrestige, labelPrestige);

            if (RollDice() >= chance)
            {
                // rapper decide to stay in the label
                return;
            }

            Debug.Log($"[RAPPER AI] {rapper.Name} leave label {label.Name}");
            rapper.Label = "";

            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text       = "news_rapper_leave_label",
                TextArgs   = new[] {rapper.Name, label.Name},
                Sprite     = label.Logo,
                Popularity = rapper.Fans
            });
        }

        private static int CalcChance(float a, float b)
        {
            const int baseChance = 50;
            return Mathf.RoundToInt(baseChance + (a - b) * 10);
        }
    }
}