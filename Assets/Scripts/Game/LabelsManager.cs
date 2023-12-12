using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Interfaces;
using Data;
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

        [Space]
        [SerializeField] private int[] expToLabelsLevelUp;
        [SerializeField] private int expChangeValue = 50;
        
        [Space]
        [SerializeField] private int maxFans = 100_000_000;
        [SerializeField] private LabelsData data;

        private List<LabelInfo> _labels;
        private List<LabelInfo> _customLabels;
        private LabelInfo _playerLabel;

        private const int minLabelLevel = 0;
        private const int maxLabelLevel = 5;
        
        public void OnStart()
        {
            _labels = GameManager.Instance.Labels;
            _customLabels = GameManager.Instance.CustomLabels;
            _playerLabel = GameManager.Instance.PlayerLabel;
            
            AppendNewLabels();
            
            TimeManager.Instance.onMonthLeft += OnMonthLeft;
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
            foreach (var label in _labels)
            {
                yield return label;
            }
            
            foreach (var label in _customLabels)
            {
                yield return label;
            }
        }
        
        /// <summary>
        /// Returns label prestige from 0 to 5 stars
        /// </summary>
        public float GetLabelPrestige(LabelInfo label)
        {
            int level = label.Prestige.Value;
            int exp = label.Prestige.Exp;
            
            int expToUpHalf = expToLabelsLevelUp[level] / 2;
            float halfStar = exp > expToUpHalf ? 0.5f : 0f;

            return level + halfStar;
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
            int score = CalcRapperScore(rapper.Fans);
            float prestige = MapScoreToPrestige(score);

            // get all labels and cache prestige values
            var labels = GetAllLabels().ToArray();
            var prestigeMap = labels.ToDictionary(k => k.Name, GetLabelPrestige);

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
                UpdatePrestige(label);
                RefreshScore(label); 
            }

            if (_playerLabel != null)
            {
                RefreshScore(_playerLabel);
            }
        }

        private void UpdatePrestige(LabelInfo label)
        {
            int dice = Random.Range(-1, 2); // [-1, 0, 1]
            if (dice == 0)
                return;
            
            int level = label.Prestige.Value;
            int newExp = label.Prestige.Exp + (expChangeValue * dice);
            
            if (newExp > 0)
            {
                if (level == maxLabelLevel) 
                    return;
                
                int expToUp = expToLabelsLevelUp[level];
                if (newExp >= expToUp)
                {
                    level += 1;
                    newExp -= expToUp;
                }
            } else
            {
                if (level == minLabelLevel) 
                    return;
                
                int expToUp = expToLabelsLevelUp[level-1];
                level -= 1;
                newExp = expToUp - newExp;
            }

            label.Prestige.Value = level;
            label.Prestige.Exp = newExp;
        }

        private void RefreshScore(LabelInfo label)
        {
            var rappers = RappersManager.Instance.GetFromLabel(label.Name);
            int newScore = rappers.Sum(rapper => CalcRapperScore(rapper.Fans));
            label.Score = newScore;
        }

        private int CalcRapperScore(int rapperFans)
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
    }
}