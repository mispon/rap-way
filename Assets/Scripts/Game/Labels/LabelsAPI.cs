using System;
using System.Collections.Generic;
using System.Linq;
using Game.Labels.Desc;
using Game.Player;
using Models.Game;

namespace Game.Labels
{
    /// <summary>
    /// Music labels public API
    /// </summary>
    public partial class LabelsPackage
    {
        /// <summary>
        /// Returns label by name
        /// </summary>
        public LabelInfo Get(string labelName)
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

            if (PlayerLabel != null && PlayerLabel.Name == labelName)
            {
                PlayerLabel.Logo = imagesBank.CustomLabelAvatar;
                return PlayerLabel;
            }

            return null;
        }
        
        /// <summary>
        /// Returns full list of labels
        /// </summary>
        public IEnumerable<LabelInfo> GetAll()
        {
            var logosMap = data.Labels.ToDictionary(k => k.Name, v => v.Logo);
            
            foreach (var label in _labels)
            {
                if (logosMap.TryGetValue(label.Name, out var logo))
                {
                    label.Logo = logo != null ? logo : imagesBank.CustomLabelAvatar;
                }
                yield return label;
            }
            
            foreach (var label in _customLabels)
            {
                label.Logo = imagesBank.CustomLabelAvatar;
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
            foreach (var label in GetAll())
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
        public float GetPrestige(LabelInfo label)
        {
            return GetPrestige(label, _settings.Labels.ExpToLevelUp);
        }
        
        /// <summary>
        /// Returns label prestige from 0 to 5 stars
        /// </summary>
        public float GetPrestige(LabelInfo label, int[] expToLevelUp)
        {
            int level = label.Prestige.Value;
            int exp = label.Prestige.Exp;
            
            if (level == _settings.Labels.MaxLevel)
                return 5f;
            
            int half = expToLevelUp[level] / 2;
            float halfStar = exp > half ? 0.5f : 0f;

            return level + halfStar;
        }
        
        /// <summary>
        /// Convert prestige to exp value
        /// </summary>
        public ExpValue PrestigeToExp(float prestige)
        {
            int level = (int) prestige;
            int exp = 0;

            if (level < prestige)
            {
                exp = _settings.Labels.ExpToLevelUp[level] / 2 + 1;
            }
            
            return new ExpValue {Value = level, Exp = exp};
        }

        /// <summary>
        /// Refresh labels score 
        /// </summary>
        public void RefreshScore(string labelName)
        {
            var label = Get(labelName);
            RefreshScore(label);
        }

        /// <summary>
        /// Returns true if player in any game label 
        /// </summary>
        public bool IsPlayerInGameLabel()
        {
            string labelName = PlayerManager.Data.Label;
            return labelName != "" && PlayerLabel != null && labelName != PlayerLabel.Name;
        }
    }
}