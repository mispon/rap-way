using System.Linq;
using Core.Analytics;
using Enums;
using Game.Labels.Desc;
using Game.Settings;
using MessageBroker;
using MessageBroker.Messages.Labels;
using UnityEngine;
using RappersAPI = Game.Rappers.RappersPackage;

namespace Game.Labels.AI
{
    public partial class LabelsAI
    {
        private static void DoInviteRapper(LabelInfo label, GameSettings settings)
        {
            label.Cooldown = settings.Labels.InviteRapperCooldown;

            var rappers   = RappersAPI.Instance.GetAll().ToArray();
            var randomIdx = Random.Range(0, rappers.Length);

            Debug.Log($"[LABEL AI] {label.Name} send invite to rapper {rappers[randomIdx].Name}");
            AnalyticsManager.LogEvent(FirebaseGameEvents.LabelAI_InviteRapper);

            MsgBroker.Instance.Publish(new LabelInviteRapperMessage
            {
                LabelName = label.Name,
                RapperId  = rappers[randomIdx].Id
            });
        }
    }
}