using System.Collections.Generic;
using System.Linq;
using Core.OrderedStarter;
using Game.Rappers.Desc;
using Game.Settings;
using ScriptableObjects;
using UnityEngine;

namespace Game.Rappers
{
    /// <summary>
    /// MonoBehavior and Starter component of rappers package
    /// </summary>
    public partial class RappersPackage : GamePackage<RappersPackage>, IStarter
    {
        [SerializeField] private RappersData data;
        [SerializeField] private ImagesBank imagesBank;

        private List<RapperInfo> _rappers;
        private List<RapperInfo> _customRappers;
        private GameSettings     _settings;
        
        public void OnStart()
        {
            _rappers       = GameManager.Instance.Rappers;
            _customRappers = GameManager.Instance.CustomRappers;
            _settings      = GameManager.Instance.Settings;

            AppendNewRappers();
            UpdateCustomRappersIDs();
        }
        
        /// <summary>
        /// Adds new rappers to saved data from static config
        /// </summary>
        private void AppendNewRappers()
        {
            var rappersIds = _rappers
                .Select(e => e.Id)
                .ToHashSet();
            var newRappers = data.Rappers
                .Where(e => !rappersIds.Contains(e.Id))
                .ToArray();
            
            foreach (var rapperInfo in newRappers)
            {
                _rappers.Add(rapperInfo);
            }
        }
        
        /// <summary>
        /// Apply real rappers IDs updates
        /// </summary>
        private void UpdateCustomRappersIDs()
        {
            int id = _rappers.Max(e => e.Id);
            
            foreach (var rapperInfo in _customRappers)
            {
                id++;
                rapperInfo.Id = id;
            }
        }
    }
}