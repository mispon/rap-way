﻿using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Interfaces;
using Data;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Game
{
    public enum FansChangeDir
    {
        None = 0,
        Decrease = 1,
        Increase = 2
    }
    
    public class RappersManager : Singleton<RappersManager>, IStarter
    {
        [Header("Common no-avatar image")]
        [SerializeField] private Sprite customRapperAvatar;
        
        [Header("Frequency of rappers fans updates in months")]
        [SerializeField] private int fanUpdateFrequency = 2;
        
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
        /// Adds new custom rapper
        /// </summary>
        public void AddCustom(RapperInfo info)
        {
            _customRappers.Add(info);
        }
        
        /// <summary>
        /// Remove custom rapper
        /// </summary>
        public void RemoveCustom(RapperInfo info)
        {
            _customRappers.Remove(info);
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

        /// <summary>
        /// Returns any random rapper
        /// </summary>
        public RapperInfo GetRandomRapper()
        {
            int rappersTotal = _rappers.Count + _customRappers.Count;
            int dice = Random.Range(0, rappersTotal);

            if (dice < _rappers.Count)
            {
                return _rappers[dice];
            }

            dice -= _rappers.Count;
            return _customRappers[dice];
        }

        /// <summary>
        /// Returns list of all rappers
        /// </summary>
        public IEnumerable<RapperInfo> GetAllRappers()
        {
            var spitesMap = data.Rappers.ToDictionary(k => k.Id, v => v.Avatar);
            
            foreach (var rapperInfo in _rappers)
            {
                if (spitesMap.TryGetValue(rapperInfo.Id, out var avatar))
                {
                    rapperInfo.Avatar = avatar != null ? avatar : customRapperAvatar;
                }
                yield return rapperInfo;
            }
            
            foreach (var rapperInfo in _customRappers)
            {
                yield return rapperInfo;
            }
        }

        /// <summary>
        /// Returns all rappers from label
        /// </summary>
        public IEnumerable<RapperInfo> GetFromLabel(string label)
        {
            var rSource = _rappers.Where(rapperInfo => rapperInfo.Label == label);
            foreach (var rapperInfo in rSource)
            {
                yield return rapperInfo;
            }

            var crSource = _customRappers.Where(rapperInfo => rapperInfo.Label == label);
            foreach (var rapperInfo in crSource)
            {
                yield return rapperInfo;
            }
        }

        public int MaxCustomRapperID()
        {
            const int minID = 0;
            return _customRappers.Any() ? _customRappers.Max(e => e.Id) : minID;
        }

        public static int GetFansCount(RapperInfo rapper)
        {
            const int factor = 1_000_000;
            return rapper.Fans * factor;
        }

        public static int GetRapperScore(RapperInfo rapper)
        {
            const int maxRapperScore = 100;
            const int maxValuableFans = 50_000_000;
            
            int fans = rapper.IsPlayer ? rapper.Fans : GetFansCount(rapper);
            var score = Convert.ToInt32(((1f * fans) / maxValuableFans) * maxRapperScore);

            return Mathf.Min(score, maxRapperScore);
        }
        
        public static float GetRapperPrestige(RapperInfo rapper)
        {
            int score = GetRapperScore(rapper);
            
            return score switch
            {
                >= 90 => 5.0f,
                >= 80 => 4.5f,
                >= 70 => 4.0f,
                >= 60 => 3.5f,
                >= 50 => 3.0f,
                >= 40 => 2.5f,
                >= 30 => 2.0f,
                >= 20 => 1.5f,
                >= 10 => 1.0f,
                >= 5 => 0.5f,
                _ => 0
            };
        }

        public bool IsNameAlreadyTaken(string nickname)
        {
            foreach (var rapper in GetAllRappers())
            {
                if (string.Equals(nickname, rapper.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}