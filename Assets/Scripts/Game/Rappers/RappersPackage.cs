using System.Collections.Generic;
using System.Linq;
using Core.OrderedStarter;
using Game.Production.Desc;
using Game.Rappers.Desc;
using Game.Settings;
using ScriptableObjects;
using UnityEngine;

namespace Game.Rappers
{
    /// <summary>
    ///     MonoBehavior and Starter component of rappers package
    /// </summary>
    public partial class RappersPackage : GamePackage<RappersPackage>, IStarter
    {
        [SerializeField] private RappersData       data;
        [SerializeField] private ConcertPlacesData concertData;
        [SerializeField] private ImagesBank        imagesBank;

        private static List<RapperInfo> _rappers       => GameManager.Instance.Rappers;
        private static List<RapperInfo> _customRappers => GameManager.Instance.CustomRappers;
        private static GameSettings     _settings      => GameManager.Instance.Settings;

        public void OnStart()
        {
            UpdateInGameRappers();
            UpdateCustomRappersIDs();

            InitAll();
            RegisterHandlers();
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
                } else
                {
                    // or append new in-game rapper
                    _rappers.Add(dr);
                }
            }
        }

        private static void UpdateCustomRappersIDs()
        {
            var id = _rappers.Max(e => e.Id);

            foreach (var rapperInfo in _customRappers)
            {
                id++;
                rapperInfo.Id = id;
            }
        }

        private void InitAll()
        {
            foreach (var rapperInfo in GetAll())
            {
                // fix fans count
                if (rapperInfo.Fans < 100)
                {
                    rapperInfo.Fans *= 1_000_000;
                }

                // create history objects
                if (rapperInfo.History == null || rapperInfo.History.Empty())
                {
                    rapperInfo.History = ProductionHistory.New;
                }
            }
        }
    }
}