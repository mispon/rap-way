using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Game.Labels.Desc;
using Models.Game;
using Random = UnityEngine.Random;
using PlayerAPI = Game.Player.PlayerPackage;

namespace Game.Labels
{
    public partial class LabelsPackage
    {
        public LabelInfo Get(string labelName)
        {
            if (string.IsNullOrEmpty(labelName))
            {
                return null;
            }

            var label = GetAll().FirstOrDefault(e => string.Equals(e.Name, labelName, StringComparison.InvariantCultureIgnoreCase));
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

        public LabelInfo GetRandom()
        {
            var labels = GetAll().ToArray();
            var dice = Random.Range(0, labels.Length);
            return labels[dice];
        }

        public IEnumerable<LabelInfo> GetAll()
        {
            foreach (var label in _labels)
            {
                if (SpritesManager.Instance.TryGetByName(label.LogoName, out var logo))
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

        public void AddCustom(LabelInfo label)
        {
            _customLabels.Add(label);
        }

        public void RemoveCustom(LabelInfo label)
        {
            _customLabels.Remove(label);
        }

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

        public float GetPrestige(LabelInfo label)
        {
            return GetPrestige(label, _settings.Labels.ExpToLevelUp);
        }

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

        public ExpValue PrestigeToExp(float prestige)
        {
            int level = (int)prestige;
            int exp = 0;

            if (level < prestige)
            {
                exp = _settings.Labels.ExpToLevelUp[level] / 2 + 1;
            }

            return new ExpValue { Value = level, Exp = exp };
        }

        public void RefreshScore(string labelName)
        {
            var label = Get(labelName);
            RefreshScore(label);
        }

        public bool IsPlayerInGameLabel()
        {
            string labelName = PlayerAPI.Data.Label;
            return labelName != "" && PlayerLabel != null && labelName != PlayerLabel.Name;
        }
    }
}