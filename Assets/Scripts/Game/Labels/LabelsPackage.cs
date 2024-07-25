using System.Collections.Generic;
using System.Linq;
using Core.Context;
using Core.OrderedStarter;
using Game.Labels.Desc;
using Game.Settings;
using ScriptableObjects;
using UnityEngine;

namespace Game.Labels
{
    /// <summary>
    /// MonoBehavior and Starter component of labels package
    /// </summary>
    public partial class LabelsPackage : GamePackage<LabelsPackage>, IStarter
    {
        [SerializeField] private LabelsData data;
        [SerializeField] private ImagesBank imagesBank;

        private List<LabelInfo> _labels;
        private List<LabelInfo> _customLabels;
        private GameSettings _settings;

        public void OnStart()
        {
            _labels = GameManager.Instance.Labels;
            _customLabels = GameManager.Instance.CustomLabels;
            _settings = GameManager.Instance.Settings;

            AppendNewLabels();
            UpdateLogoNames();
            RegisterHandlers();
        }

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

        private void UpdateLogoNames()
        {
            var logoNames = data.Labels
                .GroupBy(e => e.Name)
                .ToDictionary(k => k.Key, v => v.First().LogoName);

            foreach (var label in _labels)
            {
                label.LogoName = logoNames.TryGetValue(label.Name, out var logoName)
                    ? logoName
                    : "";
            }
        }
    }
}