using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Interfaces;
using Data;
using Game.Notifications;
using Models.Game;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Game
{
    /// <summary>
    /// Manages music labels
    /// </summary>
    public class LabelsManager : Singleton<LabelsManager>, IStarter
    {
        [SerializeField] private int rappersActionsFrequency = 1;
        [SerializeField] private int labelsActionsFrequency = 2;
        [SerializeField] private int invitePlayerFrequency = 6;

        [Space]
        [SerializeField] private int[] expToLabelsLevelUp;
        [SerializeField] private int expChangeValue = 100;
        
        [Space]
        [SerializeField] private int maxRapperValuableFans = 100_000_000;
        [SerializeField] private LabelsData data;
        [SerializeField] private Sprite customLabelsLogo;
        
        private List<LabelInfo> _labels;
        private List<LabelInfo> _customLabels;
        private LabelInfo _playerLabel;

        private const int minLabelLevel = 0;
        private const int maxLabelLevel = 5;

        public LabelInfo PlayerLabel => _playerLabel;
        public bool HasPlayerLabel => _playerLabel != null && _playerLabel.Name != "";
        
        /// <summary>
        /// Used in tests for setup internal state
        /// </summary>
        public void TestSetup(int[] expToLevelUp, int expChangeVal)
        {
            expToLabelsLevelUp = expToLevelUp;
            expChangeValue = expChangeVal;
        }
        
        public void OnStart()
        {
            _labels = GameManager.Instance.Labels;
            _customLabels = GameManager.Instance.CustomLabels;
            _playerLabel = GameManager.Instance.PlayerLabel;
            
            AppendNewLabels();
            
            TimeManager.Instance.onMonthLeft += OnMonthLeft;
        }
        
        private void OnMonthLeft()
        {
            if (TimeManager.Instance.Now.Month % rappersActionsFrequency == 0)
            {
                RandomRapperLabelAction();
            }
            
            if (TimeManager.Instance.Now.Month % labelsActionsFrequency == 0)
            {
                UpdateLabelsStats();
            }
            
            if (TimeManager.Instance.Now.Month % invitePlayerFrequency == 0)
            {
                InvitePlayerToLabel();
            }
        }

        /// <summary>
        /// Returns label by name
        /// </summary>
        public LabelInfo GetLabel(string labelName)
        {
            var label = _labels.FirstOrDefault(e => e.Name == labelName);
            if (label != null)
            {
                return label;
            }
            
            var customLabel = _customLabels.FirstOrDefault(e => e.Name == labelName);
            if (customLabel != null)
            {
                return customLabel;
            }

            if (_playerLabel != null && _playerLabel.Name == labelName)
            {
                return _playerLabel;
            }

            return null;
        }
        
        /// <summary>
        /// Returns full list of labels
        /// </summary>
        public IEnumerable<LabelInfo> GetAllLabels()
        {
            var logosMap = data.Labels.ToDictionary(k => k.Name, v => v.Logo);
            
            foreach (var label in _labels)
            {
                if (logosMap.TryGetValue(label.Name, out var logo))
                {
                    label.Logo = logo != null ? logo : customLabelsLogo;
                }
                yield return label;
            }
            
            foreach (var label in _customLabels)
            {
                label.Logo = customLabelsLogo;
                yield return label;
            }
        }

        /// <summary>
        /// Adds new custom label
        /// </summary>
        public void AddCustom(LabelInfo label)
        {
            _customLabels.Add(label);
        }
        
        /// <summary>
        /// Adds new custom label
        /// </summary>
        public void RemoveCustom(LabelInfo label)
        {
            _customLabels.Remove(label);
        }

        /// <summary>
        /// Checks if name already in use by another label
        /// </summary>
        public bool IsNameAlreadyTaken(string labelName)
        {
            foreach (var label in GetAllLabels())
            {
                if (string.Equals(label.Name, labelName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns label prestige from 0 to 5 stars
        /// </summary>
        public float GetLabelPrestige(LabelInfo label)
        {
            return GetLabelPrestige(label, expToLabelsLevelUp);
        }
        
        /// <summary>
        /// Returns label prestige from 0 to 5 stars
        /// </summary>
        public static float GetLabelPrestige(LabelInfo label, int[] expToLevelUp)
        {
            int level = label.Prestige.Value;
            int exp = label.Prestige.Exp;
            
            if (level == maxLabelLevel)
                return 5f;
            
            int half = expToLevelUp[level] / 2;
            float halfStar = exp > half ? 0.5f : 0f;

            return level + halfStar;
        }
        
        /// <summary>
        /// Convert prestige to exp value
        /// </summary>
        public ExpValue FloatToExp(float prestige)
        {
            int level = (int) prestige;
            int exp = 0;

            if (level < prestige)
            {
                exp = expToLabelsLevelUp[level] / 2 + 1;
            }
            
            return new ExpValue {Value = level, Exp = exp};
        }

        /// <summary>
        /// Appends new labels from static config
        /// </summary>
        private void AppendNewLabels()
        {
            var labelsNames = _labels.Select(e => e.Name).ToHashSet();
            
            foreach (var labelInfo in data.Labels)
            {
                if (!labelsNames.Contains(labelInfo.Name))
                {
                    _labels.Add(labelInfo);
                }
            }
        }

        /// <summary>
        /// Random rapper tries to change label
        /// </summary>
        private void RandomRapperLabelAction()
        {
            var rapper = RappersManager.Instance.GetRandomRapper();
            
            if (string.IsNullOrEmpty(rapper.Label))
            {
                RapperJoinLabelAction(rapper);
                return;
            } 
            
            RapperChangeOrLeaveLabelAction(rapper);
        }

        private void RapperJoinLabelAction(RapperInfo rapper)
        {
            int score = CalcRapperScore(rapper.Fans, maxRapperValuableFans);
            float prestige = MapScoreToPrestige(score);

            // get all labels and cache prestige values
            var labels = GetAllLabels().ToArray();
            var prestigeMap = labels.ToDictionary(
                k => k.Name, 
                GetLabelPrestige
            );

            // filter and sort labels by prestige value
            labels = labels
                .Where(e => prestige >= prestigeMap[e.Name])
                .OrderByDescending(e => prestigeMap[e.Name])
                .ToArray();
            
            if (labels.Length == 0)
                return;
            
            // pick label by weighted random algorithm
            // the more prestige labels has a higher chance
            float weightTotal = prestigeMap.Sum(pair => pair.Value);
            float dice = Random.Range(0, weightTotal);

            foreach (var label in labels)
            {
                dice -= prestigeMap[label.Name];
                if (dice >= 0)
                {
                    continue;
                }

                Debug.Log($"Rapper {rapper.Name} join to label {label.Name}");
                rapper.Label = label.Name;
                break;
            }
        }

        private void RapperChangeOrLeaveLabelAction(RapperInfo rapper)
        {
            var label = GetLabel(rapper.Label);
            if (label == null)
            {
                rapper.Label = "";
                return;
            }
            
            int decisionThreshold = label.IsPlayer ? 25 : 50;
            int decisionDice = Random.Range(0, 100);
            
            if (decisionDice >= decisionThreshold)
                // negative decision, do nothing
                return;
            
            int actionDice = Random.Range(0, 2); // 0 - leave, 1 - change
            if (actionDice == 0)
            {
                Debug.Log($"Rapper {rapper.Name} leave from label {label.Name}");
                rapper.Label = "";
                return;
            }

            RapperJoinLabelAction(rapper);
        }

        /// <summary>
        /// Update and recalculate labels stats
        /// </summary>
        private void UpdateLabelsStats()
        {
            var labels = GetAllLabels();
            foreach (var label in labels)
            {
                int dice = Random.Range(-1, 2); // [-1, 0, 1]
                UpdatePrestige(label, dice);
                RefreshScore(label); 
            }

            if (_playerLabel != null)
            {
                RefreshScore(_playerLabel);
            }
        }

        private void UpdatePrestige(LabelInfo label, int dice)
        {
            if (dice == 0)
                return;
            
            int level = label.Prestige.Value;
            int newExp = label.Prestige.Exp + (expChangeValue * dice);
            
            if (newExp > 0)
            {
                if (level != maxLabelLevel)
                {
                    int expToUp = expToLabelsLevelUp[level];
                    if (newExp >= expToUp)
                    {
                        level += 1;
                        newExp -= expToUp;
                    }
                }
            } else
            {
                if (level == minLabelLevel) 
                    return;
                
                int expToUp = expToLabelsLevelUp[level-1];
                level -= 1;
                newExp = expToUp + newExp;
            }

            label.Prestige.Value = level;
            label.Prestige.Exp = newExp;
            Debug.Log($"{label.Name}: {label.Prestige}");
        }

        private void RefreshScore(LabelInfo label)
        {
            var rappers = RappersManager.Instance.GetFromLabel(label.Name);
            int newScore = rappers.Sum(rapper => CalcRapperScore(rapper.Fans, maxRapperValuableFans));
            label.Score = newScore;
            Debug.Log($"{label.Name} new score: {newScore}");
        }

        private static int CalcRapperScore(int rapperFans, int maxFans)
        {
            const int maxRapperScore = 100;
            var score = Convert.ToInt32(1f * rapperFans / maxFans * maxRapperScore);
            return Mathf.Min(score, maxRapperScore);
        }

        private static float MapScoreToPrestige(int score)
        {
            return score switch
            {
                >= 90 => 5.0f,
                >= 80 => 4.5f,
                >= 70 => 4.0f,
                >= 60 => 3.5f,
                >= 50 => 3.0f,
                >= 40 => 2.5f,
                >= 30 => 2.0f,
                >= 20 => 1.5f,
                >= 10 => 1.0f,
                >= 5 => 0.5f,
                _ => 0
            };
        }

        /// <summary>
        /// Invites player to random available label
        /// </summary>
        private void InvitePlayerToLabel()
        {
            if (PlayerManager.Data.Label != "")
                // already in label
                return;
            
            int score = CalcRapperScore(PlayerManager.Data.Fans, maxRapperValuableFans);
            float prestige = MapScoreToPrestige(score);
            
            var labels = GetAllLabels()
                .Where(e => prestige >= GetLabelPrestige(e))
                .ToArray();

            var randomIdx = Random.Range(0, labels.Length);
            var label = labels[randomIdx];
            
            NotificationManager.Instance.AddClickNotification(() =>
            {
                // open label's contract page
                Debug.Log($"{label.Name} invites you!");
            });
        }
    }
}