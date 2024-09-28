using System.Collections.Generic;
using System.Linq;
using Core.OrderedStarter;
using Game.Rappers.AI;
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
        [SerializeField] private RappersAI rapperAITemplate;
        [SerializeField] private RappersData data;
        [SerializeField] private ImagesBank imagesBank;

        private List<RapperInfo> _rappers => GameManager.Instance.Rappers;
        private List<RapperInfo> _customRappers => GameManager.Instance.CustomRappers;
        private GameSettings _settings => GameManager.Instance.Settings;

        public void OnStart()
        {
            UpdateInGameRappers();
            UpdateCustomRappersIDs();

            LoadRappersAI();
        }

        private void UpdateInGameRappers()
        {
            var rapperSet = _rappers.ToDictionary(e => e.Id);

            foreach (var dr in data.Rappers)
            {
                if (rapperSet.TryGetValue(dr.Id, out var rapper))
                {
                    // setup avatar for existing
                    rapper.AvatarName = dr.AvatarName;
                }
                else
                {
                    // or append new in-game rapper
                    _rappers.Add(dr);
                }
            }
        }

        private void UpdateCustomRappersIDs()
        {
            int id = _rappers.Max(e => e.Id);

            foreach (var rapperInfo in _customRappers)
            {
                id++;
                rapperInfo.Id = id;
            }
        }

        private void LoadRappersAI()
        {
            foreach (var rapperInfo in GetAll())
            {
                Instantiate(rapperAITemplate, transform).Init(rapperInfo);
            }
        }
    }
}