using System.Linq;
using Game.Labels.Desc;
using Game.Notifications;
using Game.Rappers.Desc;
using MessageBroker;
using MessageBroker.Messages.UI;
using UI.Enums;
using UnityEngine;
using RappersAPI = Game.Rappers.RappersPackage;
using PlayerAPI = Game.Player.PlayerPackage;

namespace Game.Labels
{
    /// <summary>
    /// Game labels life cycle
    /// </summary>
    public partial class LabelsPackage
    {
        /// <summary>
        /// Update and recalculate labels stats
        /// </summary>
        private void UpdateLabelsStats()
        {
            var labels = GetAll();
            foreach (var label in labels)
            {
                int dice = Random.Range(-1, 2); // [-1, 0, 1]
                UpdatePrestige(label, dice);
                RefreshScore(label); 
            }

            var playerLabel = GameManager.Instance.PlayerLabel;
            if (playerLabel != null)
            {
                RefreshScore(playerLabel);
            }
        }
        
        /// <summary>
        /// Updates labels prestige
        /// </summary>
        private void UpdatePrestige(LabelInfo label, int dice)
        {
            if (dice == 0)
                return;
            
            int level = label.Prestige.Value;
            int newExp = label.Prestige.Exp + _settings.Labels.ExpChangeValue * dice;
            
            if (newExp > 0)
            {
                if (level != _settings.Labels.MaxLevel)
                {
                    int expToUp = _settings.Labels.ExpToLevelUp[level];
                    if (newExp >= expToUp)
                    {
                        level += 1;
                        newExp -= expToUp;
                    }
                }
            } else
            {
                if (level == _settings.Labels.MinLevel) 
                    return;
                
                int expToUp = _settings.Labels.ExpToLevelUp[level-1];
                level -= 1;
                newExp = expToUp + newExp;
            }

            label.Prestige.Value = level;
            label.Prestige.Exp = newExp;
            
            // labels production depends on prestige 
            label.Production.Value = Mathf.Max(1, level);
        }
        
        /// <summary>
        /// Refresh labels score 
        /// </summary>
        private static void RefreshScore(LabelInfo label)
        {
            var rappers = RappersAPI.Instance.GetFromLabel(label.Name);
            int newScore = rappers.Sum(RappersAPI.GetRapperScore);
            label.Score = newScore;
        }
        
        /// <summary>
        /// Random rapper tries to change label
        /// </summary>
        private void RandomRapperLabelAction()
        {
            var rapper = RappersAPI.Instance.GetRandomRapper();
            
            if (rapper.Label == "")
            {
                RapperJoinLabelAction(rapper);
                return;
            } 
            
            RapperChangeOrLeaveLabelAction(rapper);
        }
        
        /// <summary>
        /// Handle rapper's join action to one of labels
        /// </summary>
        private void RapperJoinLabelAction(RapperInfo rapper)
        {
            float prestige = RappersAPI.GetRapperPrestige(rapper);

            // get all labels and cache prestige values
            var labels = GetAll().ToArray();
            var prestigeMap = labels.ToDictionary(
                k => k.Name, 
                GetPrestige
            );

            // filter and sort labels by prestige value
            labels = labels
                .Where(e => prestige >= prestigeMap[e.Name] && (prestige - prestigeMap[e.Name]) <= 1f)
                .OrderBy(e => prestigeMap[e.Name])
                .ToArray();
            
            if (labels.Length == 0)
                return;
            
            // pick label by weighted random algorithm
            // the more prestige labels has a higher chance
            float weightTotal = labels.Sum(e => prestigeMap[e.Name]);
            float dice = Random.Range(0, weightTotal);

            foreach (var label in labels)
            {
                dice -= prestigeMap[label.Name];
                if (dice >= 0)
                {
                    continue;
                }
                
                rapper.Label = label.Name;
                RefreshScore(label);
                break;
            }
        }

        /// <summary>
        /// Handle rapper's leave or change action from current label
        /// </summary>
        private void RapperChangeOrLeaveLabelAction(RapperInfo rapper)
        {
            var label = Get(rapper.Label);
            if (label == null)
            {
                rapper.Label = "";
                return;
            }
            
            int decisionThreshold = label.IsPlayer ? 20 : 50;
            int decisionDice = Random.Range(0, 100);
            
            if (decisionDice >= decisionThreshold)
                // negative decision, do nothing
                return;
            
            int actionDice = Random.Range(0, 2); // 0 - leave, 1 - change
            if (actionDice == 0)
            {
                rapper.Label = "";
                RefreshScore(label);
                return;
            }

            RapperJoinLabelAction(rapper);
        }
        
        /// <summary>
        /// Invites player to random available label
        /// </summary>
        private void InvitePlayerToLabel()
        {
            if (PlayerAPI.Data.Label != "")
                // already in label
                return;

            if (PlayerAPI.Data.Fans < 50_000)
            {
                // too low count of fans
                return;
            }

            var rapper = new RapperInfo {Fans = PlayerAPI.Data.Fans, IsPlayer = true}; 
            float prestige = RappersAPI.GetRapperPrestige(rapper);
            
            var labels = GetAll()
                .Where(e => prestige >= GetPrestige(e))
                .ToArray();

            if (labels.Length == 0)
                return;
            
            var randomIdx = Random.Range(0, labels.Length);
            var label = labels[randomIdx];
            
            NotificationManager.Instance.AddClickNotification(() =>
            {
                MsgBroker.Instance.Publish(new WindowControlMessage
                {
                    Type = WindowType.LabelContract,
                    Context = label
                });
            });
        }
    }
}