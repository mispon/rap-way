using System.Linq;
using Game.Labels.Desc;
using Game.Rappers.Desc;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using MessageBroker.Messages.UI;
using UI.Enums;
using UnityEngine;
using RappersAPI = Game.Rappers.RappersPackage;
using PlayerAPI = Game.Player.PlayerPackage;
using Enums;

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

            var level = label.Prestige.Value;
            var newExp = label.Prestige.Exp + _settings.Labels.ExpChangeValue * dice;

            if (newExp > 0)
            {
                if (level != _settings.Labels.MaxLevel)
                {
                    var expToUp = _settings.Labels.ExpToLevelUp[level];
                    if (newExp >= expToUp)
                    {
                        level += 1;
                        newExp -= expToUp;
                    }
                }
            }
            else
            {
                if (level == _settings.Labels.MinLevel)
                    return;

                var expToUp = _settings.Labels.ExpToLevelUp[level - 1];
                level -= 1;
                newExp = expToUp + newExp;
            }

            label.Prestige.Value = level;
            label.Prestige.Exp = newExp;

            // labels production depends on prestige 
            label.Production.Value = Mathf.Max(1, level);
        }

        private static void RefreshScore(LabelInfo label)
        {
            var rappers = RappersAPI.Instance.GetFromLabel(label.Name);
            var newScore = rappers.Sum(RappersAPI.GetRapperScore);
            label.Score = newScore;
        }

        private void RandomRapperLabelAction()
        {
            var rapper = RappersAPI.Instance.GetRandom();

            if (rapper.Label == "")
            {
                RapperJoinLabelAction(rapper);
                return;
            }

            RapperChangeOrLeaveLabelAction(rapper);
        }

        private void RapperJoinLabelAction(RapperInfo rapper)
        {
            var prestige = RappersAPI.GetRapperPrestige(rapper);

            // get all labels and cache prestige values
            var labels = GetAll().ToArray();
            var prestigeMap = labels.ToDictionary(
                k => k.Name,
                GetPrestige
            );

            // filter and sort labels by prestige value
            labels = labels
                .Where(e => prestige >= prestigeMap[e.Name] && prestige - prestigeMap[e.Name] <= 1f)
                .OrderBy(e => prestigeMap[e.Name])
                .ToArray();

            if (labels.Length == 0)
            {
                return;
            }

            // pick label by weighted random algorithm
            // the more prestige labels has a higher chance
            var weightTotal = labels.Sum(e => prestigeMap[e.Name]);
            var dice = Random.Range(0, weightTotal);

            foreach (var label in labels)
            {
                dice -= prestigeMap[label.Name];
                if (dice >= 0)
                    continue;


                MsgBroker.Instance.Publish(new NewsMessage
                {
                    Text = "news_rapper_join_label",
                    TextArgs = new[] {
                        rapper.Name,
                        label.Name
                    },
                    Sprite = rapper.Avatar,
                    Popularity = PlayerAPI.Data.Fans
                });

                rapper.Label = label.Name;
                RefreshScore(label);
                break;
            }
        }

        private void RapperChangeOrLeaveLabelAction(RapperInfo rapper)
        {
            var label = Get(rapper.Label);
            if (label == null)
            {
                rapper.Label = "";
                return;
            }

            var decisionThreshold = label.IsPlayer ? 20 : 50;
            var decisionDice = Random.Range(0, 100);

            if (decisionDice >= decisionThreshold)
            {
                // negative decision, do nothing
                return;
            }

            var actionDice = Random.Range(0, 2); // 0 - leave, 1 - change
            if (actionDice == 0)
            {
                MsgBroker.Instance.Publish(new NewsMessage
                {
                    Text = "news_rapper_leave_label",
                    TextArgs = new[] {
                        rapper.Name,
                        label.Name
                    },
                    Sprite = rapper.Avatar,
                    Popularity = PlayerAPI.Data.Fans
                });

                rapper.Label = "";
                RefreshScore(label);
                return;
            }

            RapperJoinLabelAction(rapper);
        }

        private void InvitePlayerToLabel()
        {
            if (PlayerAPI.Data.Label != "")
            {
                // already in label
                return;
            }

            if (PlayerAPI.Data.Fans < 50_000)
            {
                // too low count of fans
                return;
            }

            var rapper = new RapperInfo { Fans = PlayerAPI.Data.Fans, IsPlayer = true };
            var prestige = RappersAPI.GetRapperPrestige(rapper);

            var labels = GetAll()
                .Where(e => prestige >= GetPrestige(e))
                .ToArray();

            if (labels.Length == 0)
                return;

            // select random label 
            var randomIdx = Random.Range(0, labels.Length);
            var label = labels[randomIdx];

            // and send invite message
            MsgBroker.Instance.Publish(new EmailMessage
            {
                Type = EmailsType.LabelsContract,
                Title = "label_contract_greeting",
                TitleArgs = new[] { label.Name },
                Content = "label_contract_text",
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
                        Type = WindowType.LabelContract,
                        Context = label
                    });
                }
            });
        }
    }
}