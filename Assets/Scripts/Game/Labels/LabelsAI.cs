using System.Collections.Generic;
using System.Linq;
using Game.Labels.AI;
using Game.Labels.Desc;
using UnityEngine;

namespace Game.Labels
{
    public partial class LabelsPackage
    {
        private readonly LabelsAI _labelsAI = new();

        private void TriggerAIAction()
        {
            var labels = GetAll().ToArray();
            DecrementCooldowns(labels);

            // one random label do action per week
            var randomIdx = Random.Range(0, labels.Length);
            _labelsAI.DoAction(labels[randomIdx], _settings);
        }

        private static void DecrementCooldowns(IEnumerable<LabelInfo> labels)
        {
            foreach (var label in labels)
            {
                if (label.Cooldown > 0)
                {
                    label.Cooldown--;
                }
            }
        }
    }
}