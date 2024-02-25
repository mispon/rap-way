using System.Collections.Generic;
using System.Linq;
using Core.OrderedStarter;
using Game.Labels.Desc;
using Game.Settings;
using ScriptableObjects;
using UI.Windows.Pages.Contracts;
using UI.Windows.Pages.Personal;
using UnityEngine;

namespace Game.Labels
{
    public partial class LabelsPackage : IStarter
    {
        [SerializeField] private LabelContractPage contractPage;
        [SerializeField] private PersonalPage personalPage;
        [Space]
        [SerializeField] private LabelsData data;
        [SerializeField] private ImagesBank imagesBank;
        
        private List<LabelInfo> _labels;
        private List<LabelInfo> _customLabels;
        private GameSettings    _settings;
        
        public void OnStart()
        {
            _labels       = GameManager.Instance.Labels;
            _customLabels = GameManager.Instance.CustomLabels;
            _settings     = GameManager.Instance.Settings;
            
            AppendNewLabels();
            RegisterHandlers();
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
    }
}