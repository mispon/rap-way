using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using Data;
using Game;
using UnityEngine;
using Utils;

namespace Core
{
    public enum FansChangeDir
    {
        None = 0,
        Decrease = 1,
        Increase = 2
    }
    
    public class RappersManager : Singleton<RappersManager>, IStarter
    {
        [Header("Frequency of rappers fans updates in months")]
        [SerializeField] private int fanUpdateFrequency = 2;
        
        [Header("Frequency of rappers labels updates in months")]
        [SerializeField] private int labelsUpdateFrequency = 4;

        [Header("Fans change limits")]
        [SerializeField] private int minRapperFans = 1;
        [SerializeField] private int maxRapperFans = 500_000_000;
        
        [Space]
        [SerializeField] private RappersData data;

        private List<RapperInfo> _rappers;
        private List<RapperInfo> _customRappers;

        public void OnStart()
        {
            _rappers = GameManager.Instance.Rappers;
            _customRappers = GameManager.Instance.CustomRappers;

            AppendNewRappers();
            UpdateCustomRappersIDs();
           
            TimeManager.Instance.onMonthLeft += OnMonthLeft;
        }

        /// <summary>
        /// Adds new rappers to saved data from static config
        /// </summary>
        private void AppendNewRappers()
        {
            var rappersIds = _rappers
                .Select(e => e.Id)
                .ToHashSet();
            
            var newRappers = data.Rappers.Where(e => !rappersIds.Contains(e.Id)).ToArray();
            
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
        
        private void OnMonthLeft()
        {
            if (TimeManager.Instance.Now.Month % fanUpdateFrequency == 0)
            {
                UpdateRappersFans();
            }
            if (TimeManager.Instance.Now.Month % labelsUpdateFrequency == 0)
            {
                UpdateRappersLabels();
            }
        }

        private void UpdateRappersFans()
        {
            foreach (var rapperInfo in GetAllRappers())
            {
                var dice = Random.Range(0, 3);
                if (dice == (int) FansChangeDir.None)
                {
                    continue;
                }

                var value = dice == (int) FansChangeDir.Increase ? 1 : -1;
                rapperInfo.Fans = Mathf.Clamp(rapperInfo.Fans + value, minRapperFans, maxRapperFans);
            }
        }

        private void UpdateRappersLabels()
        {
            // todo:
        }

        public IEnumerable<RapperInfo> GetAllRappers()
        {
            var spitesMap = data.Rappers.ToDictionary(k => k.Id, v => v.Avatar);
            
            foreach (var rapperInfo in _rappers)
            {
                if (spitesMap.TryGetValue(rapperInfo.Id, out var avatar))
                {
                    rapperInfo.Avatar = avatar;
                }
                yield return rapperInfo;
            }
            
            foreach (var rapperInfo in _customRappers)
            {
                yield return rapperInfo;
            }
        }

        public int MaxCustomRapperID()
        {
            return _customRappers.Max(e => e.Id);
        }
    }
}