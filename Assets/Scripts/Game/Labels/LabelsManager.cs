using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.OrderedStarter;
using Game.Notifications;
using Game.Player;
using Game.Rappers;
using Game.Time;
using Models.Game;
using ScriptableObjects;
using UI.Windows.Pages.Contracts;
using UI.Windows.Pages.Personal;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Labels
{
    /// <summary>
    /// Manages music labels
    /// </summary>
    public class LabelsManager : Singleton<LabelsManager>, IStarter
    {
        [SerializeField] private int rappersActionsFrequency = 1;
        [SerializeField] private int invitePlayerFrequency = 3;

        [Space]
        [SerializeField] private int[] expToLabelsLevelUp;
        [SerializeField] private int expChangeValue = 100;
        
        [Space]
        [SerializeField] private LabelsData data;
        [SerializeField] private Sprite customLabelsLogo;

        [Space]
        [SerializeField] private LabelContractPage contractPage;
        [SerializeField] private PersonalPage personalPage;
        
        
        private List<LabelInfo> _labels;
        private List<LabelInfo> _customLabels;
        private LabelInfo _playerLabel;

        private const int minLabelLevel = 0;
        private const int maxLabelLevel = 5;

        public LabelInfo PlayerLabel => _playerLabel;
        public bool HasPlayerLabel => _playerLabel != null && !string.IsNullOrWhiteSpace(_playerLabel.Name);
        
        public void OnStart()
        {
            _labels = GameManager.Instance.Labels;
            _customLabels = GameManager.Instance.CustomLabels;
            _playerLabel = GameManager.Instance.PlayerLabel;
            
            AppendNewLabels();
            
            TimeManager.Instance.onMonthLeft += OnMonthLeft;
            TimeManager.Instance.onWeekLeft += OnWeekLeft;
        }
        
        private void OnMonthLeft()
        {
            if (TimeManager.Instance.Now.Month % rappersActionsFrequency == 0)
            {
                RandomRapperLabelAction();
            }
            
            if (TimeManager.Instance.Now.Month % invitePlayerFrequency == 0)
            {
                InvitePlayerToLabel();
            }

            SendPlayersLabelIncomeNotification();
        }

        private void OnWeekLeft()
        {
            UpdateLabelsStats();
        }

        /// <summary>
        /// Returns label by name
        /// </summary>
        public LabelInfo GetLabel(string labelName)
        {
            if (string.IsNullOrEmpty(labelName))
                return null;
            
            var label = _labels.FirstOrDefault(e => string.Equals(e.Name, labelName, StringComparison.InvariantCultureIgnoreCase));
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
                _playerLabel.Logo = customLabelsLogo;
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
            
            if (rapper.Label == "")
            {
                RapperJoinLabelAction(rapper);
                return;
            } 
            
            RapperChangeOrLeaveLabelAction(rapper);
        }

        private void RapperJoinLabelAction(RapperInfo rapper)
        {
            float prestige = RappersManager.GetRapperPrestige(rapper);

            // get all labels and cache prestige values
            var labels = GetAllLabels().ToArray();
            var prestigeMap = labels.ToDictionary(
                k => k.Name, 
                GetLabelPrestige
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

        private void RapperChangeOrLeaveLabelAction(RapperInfo rapper)
        {
            var label = GetLabel(rapper.Label);
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
            // labels production depends on prestige 
            label.Production.Value = Mathf.Max(1, level);
        }

        public void RefreshScore(string labelName)
        {
            var label = GetLabel(labelName);
            RefreshScore(label);
        }
        
        private static void RefreshScore(LabelInfo label)
        {
            var rappers = RappersManager.Instance.GetFromLabel(label.Name);
            int newScore = rappers.Sum(RappersManager.GetRapperScore);
            label.Score = newScore;
        }

        /// <summary>
        /// Invites player to random available label
        /// </summary>
        private void InvitePlayerToLabel()
        {
            if (PlayerManager.Data.Label != "")
                // already in label
                return;

            if (PlayerManager.Data.Fans < 50_000)
            {
                // too low count of fans
                return;
            }

            var rapper = new RapperInfo {Fans = PlayerManager.Data.Fans, IsPlayer = true}; 
            float prestige = RappersManager.GetRapperPrestige(rapper);
            
            var labels = GetAllLabels()
                .Where(e => prestige >= GetLabelPrestige(e))
                .ToArray();

            if (labels.Length == 0)
                return;
            
            var randomIdx = Random.Range(0, labels.Length);
            var label = labels[randomIdx];
            
            NotificationManager.Instance.AddClickNotification(() =>
            {
                contractPage.Show(label);
            });
        }

        /// <summary>
        /// Returns label income from it's members
        /// </summary>
        public int GetPlayersLabelIncome()
        {
            if (_playerLabel == null || _playerLabel.Name == "")
                return 0;
            
            var members = RappersManager.Instance
                .GetAllRappers()
                .Where(e => string.Equals(e.Label, _playerLabel.Name, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            const int incomePercent = 5;
            return members.Sum(e => RappersManager.GetFansCount(e) / 100 * incomePercent);
        }

        /// <summary>
        /// Creates player's label
        /// </summary>
        public void CreatePlayersLabel(LabelInfo label)
        {
            _playerLabel = label;
            GameManager.Instance.PlayerLabel = label;
        }
        
        /// <summary>
        /// Removes player's label
        /// </summary>
        public void DisbandPlayersLabel()
        {
            _playerLabel = null;
            GameManager.Instance.PlayerLabel = null;
        }

        private void SendPlayersLabelIncomeNotification()
        {
            if (!HasPlayerLabel)
                return;
            
            NotificationManager.Instance.AddClickNotification(() =>
            {
                personalPage.ShowLabelMoneyReport();
            });
        }

        public bool IsPlayerInGameLabel()
        {
            string labelName = PlayerManager.Data.Label;
            return labelName != "" && labelName != _playerLabel.Name;
        }
    }
}