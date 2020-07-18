using System;
using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using Data;
using Enums;
using Game;
using JetBrains.Annotations;
using Models.Info.Production;
using Models.Player;
using UnityEngine;
using Utils;

namespace Core
{
    /// <summary>
    /// Менеджер Ачивок.
    /// </summary>
    public class AchievementsManager: Singleton<AchievementsManager>, IStarter
    {
        [Header("Данные")]
        [SerializeField] private AchievementsData achievementsData;
        [SerializeField] private ConcertPlacesData concertPlacesData;

        /// <summary>
        /// На каждое событие изменения одной из сущностей вешается листенер, который выбирает все НЕРАЗБЛОКИРОВАННЫЕ AchievementInfo конкретного AchievementsType.
        /// В зависимсоти от логики ищется первое отсортированное по CompareValue или же все , прошедшие условия.
        /// Найденное отображается в UI
        /// </summary>
        public void OnStart()
        {
            achievementsData.Initialize();

            PlayerManager.Instance.onMoneyAdd += CheckMoney;
            PlayerManager.Instance.onFansAdd += CheckFans;
            PlayerManager.Instance.onHypeAdd += CheckHype;

            ProductionManager.Instance.onTrackAdd += CheckTrackChartPosition;
            ProductionManager.Instance.onAlbumAdd += CheckAlbumChartPosition;
            ProductionManager.Instance.onClipAdd += CheckClipLoser;
            ProductionManager.Instance.onConcertAdd += CheckConcertPlace;
            
            //CheckFeat
            //CheckBattle
        }
        
        private void CheckMoney(int money)
        {
            BaseCheckValue(AchievementsType.Money, money);
        }
        
        private void CheckFans(int fans)
        {
            BaseCheckValue(AchievementsType.Fans, fans);
        }
        
        private void CheckHype(int hype)
        {
            BaseCheckValue(AchievementsType.HypeBeast, hype);
        }
        
        private void CheckTrackChartPosition(TrackInfo trackInfo)
        {
            MultipleCheckValue(AchievementsType.TrackChartPosition, trackInfo.ChartPosition,
                info => info.Achievement.CompareValue);
        }

        private void CheckAlbumChartPosition(AlbumInfo albumInfo)
        {
            MultipleCheckValue(AchievementsType.AlbumChartPosition, albumInfo.ChartPosition,
                info => info.Achievement.CompareValue);
        }
        
        private void CheckClipLoser(ClipInfo clipInfo)
        {
            BaseCheckValue(AchievementsType.ClipLoser, clipInfo.Dislikes);
        }

        private void CheckConcertPlace(ConcertInfo concertInfo)
        {
            var index = concertPlacesData.Places.ToList()
                .IndexOf(concertPlacesData.Places.First(pl => pl.NameKey == concertInfo.LocationName));
            EqualCheckValue(AchievementsType.ConcertPlace, index);
        }

        private void CheckFeat(int raperId)
        {
            EqualCheckValue(AchievementsType.Feat, raperId);
        }

        private void CheckBattle(int raperId)
        {
            EqualCheckValue(AchievementsType.Battle, raperId);
        }
        
        /// <summary>
        /// Базовая функция проверки выполнения условия достижения.
        /// Не рассматривает возможность получения сразу нескольких достижений одного типа
        /// </summary>
        private void BaseCheckValue(AchievementsType type, int value)
        {
            var lockedInfos = achievementsData.LockedInfos.Where(info => info.Achievement.Type == type);
            if (!lockedInfos.Any())
                return;
            
            var achievementInfo = lockedInfos.OrderBy(info => info.Achievement.CompareValue).First();
            if (achievementInfo.CheckCondition(value, achievementInfo.Achievement.CompareValue))
                AddAchievement(achievementInfo.Achievement);
        }

        /// <summary>
        /// Ищет все новые заработанные достижения указанного типа, фиксирует все в PlayerHistory,
        /// а выводит в UI лишь первое, согласно ключу сортировки
        /// </summary>
        private void MultipleCheckValue(AchievementsType type, int value, Func<AchievementInfo, int> orderSelector)
        {
            var lockedInfos = achievementsData.LockedInfos.Where(info => info.Achievement.Type == type);
            if (!lockedInfos.Any())
                return;

            var achievementInfos =
                lockedInfos.Where(info => info.CheckCondition(value, info.Achievement.CompareValue));

            if (achievementInfos.Any())
            {
                var newUnlockedInfos = achievementInfos.OrderBy(orderSelector).ToArray();
                for (int i = 0; i < newUnlockedInfos.Length; i++)
                    AddAchievement(newUnlockedInfos[i].Achievement, i == 0);
            }
        }
        
        /// <summary>
        /// Базовая функция проверки выполнения условия достижения на точное совпадение
        /// </summary>
        private void EqualCheckValue(AchievementsType type, int value)
        {
            var lockedInfos = achievementsData.LockedInfos.Where(info => info.Achievement.Type == type);
            if (!lockedInfos.Any())
                return;

            var achievementInfo =
                lockedInfos.FirstOrDefault(info => info.CheckCondition(value, info.Achievement.CompareValue));
            
            if(achievementInfo != default)
                AddAchievement(achievementInfo.Achievement);
        }

        /// <summary>
        /// Функция добавления ачивки в список заработанных
        /// </summary>
        private static void AddAchievement(Achievement achievement, bool showUi = true)
        {
            achievement.Unlocked = true;
            PlayerManager.Data.Achievements.Add(achievement);

            if (showUi)
            {
                //todo: show achievement UI
                Debug.Log($"Show UI: {achievement.Type} | {achievement.CompareValue}");
            }
        }
    }
}