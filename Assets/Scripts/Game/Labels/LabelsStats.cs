using System.Linq;
using Game.Labels.Desc;
using UnityEngine;
using RappersAPI = Game.Rappers.RappersPackage;

namespace Game.Labels
{
    public partial class LabelsPackage
    {
        private void UpdateLabelsStats()
        {
            var labels = GetAll();
            foreach (var label in labels)
            {
                var dice = Random.Range(-1, 2); // [-1, 0, 1]
                UpdatePrestige(label, dice);
                RefreshScore(label);
            }

            var playerLabel = GameManager.Instance.PlayerLabel;
            if (playerLabel != null)
            {
                RefreshScore(playerLabel);
            }
        }

        private void UpdatePrestige(LabelInfo label, int dice)
        {
            if (dice == 0)
            {
                return;
            }

            var level  = label.Prestige.Value;
            var newExp = label.Prestige.Exp + _settings.Labels.ExpChangeValue * dice;

            if (newExp > 0)
            {
                if (level != _settings.Labels.MaxLevel)
                {
                    var expToUp = _settings.Labels.ExpToLevelUp[level];
                    if (newExp >= expToUp)
                    {
                        level  += 1;
                        newExp -= expToUp;
                    }
                }
            } else
            {
                if (level == _settings.Labels.MinLevel)
                {
                    return;
                }

                var expToUp = _settings.Labels.ExpToLevelUp[level - 1];
                level  -= 1;
                newExp =  expToUp + newExp;
            }

            label.Prestige.Value = level;
            label.Prestige.Exp   = newExp;

            // labels production depends on prestige 
            label.Production.Value = Mathf.Max(1, level);
        }

        private static void RefreshScore(LabelInfo label)
        {
            var rappers  = RappersAPI.Instance.GetFromLabel(label.Name);
            var newScore = rappers.Sum(RappersAPI.GetRapperScore);
            label.Score = newScore;
        }
    }
}