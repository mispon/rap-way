using System.Collections.Generic;
using System.Linq;
using Core.OrderedStarter;
using Game.Labels.Desc;
using Game.Settings;
using ScriptableObjects;
using UnityEngine;

namespace Game.Labels
{
    public partial class LabelsPackage : GamePackage<LabelsPackage>, IStarter
    {
        [SerializeField] private LabelsData data;
        [SerializeField] private ImagesBank imagesBank;

        private List<LabelInfo> _labels       => GameManager.Instance.Labels;
        private List<LabelInfo> _customLabels => GameManager.Instance.CustomLabels;
        private GameSettings    _settings     => GameManager.Instance.Settings;

        public void OnStart()
        {
            UpdateInGameLabels();
            UpdateLogoNames();

            RegisterHandlers();
        }

        private void UpdateInGameLabels()
        {
            var labelsSet = _labels.ToDictionary(e => e.Name);

            foreach (var dl in data.Labels)
            {
                if (labelsSet.TryGetValue(dl.Name, out var label))
                {
                    // update logo for existing
                    label.LogoName = dl.LogoName;
                } else
                {
                    // or add new label
                    _labels.Add(dl);
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