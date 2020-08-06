using System;
using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using Data;
using Enums;
using Game;
using Game.Effects;
using Game.Notifications;
using Game.Pages.Achievement;
using JetBrains.Annotations;
using Localization;
using Models.Info.Production;
using Models.Player;
using UnityEngine;
using Utils;
using Utils.Extensions;

namespace Core
{
    /// <summary>
    /// Менеджер Ачивок.
    /// </summary>
    public class AchievementsManager: Singleton<AchievementsManager>, IStarter
    {
        [Header("Эффект открытия новой шмотки")] 
        [SerializeField, Tooltip("Базовая картинка для отображения в уведомлении")] 
        private Sprite newAchievementSprite;
        [SerializeField] private NewItemEffect newAchievementEffect;
        
        [Header("Страница новых достижений")]
        [SerializeField] private NewAchievementsPage newAchievementsPage;

        [Header("Данные")]
        [SerializeField] private AchievementsData achievementsData;

        /// <summary>
        /// Место проведения последнего концерта
        /// </summary>
        private string _lastConcertPlaceName;
        

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
            PlayerManager.Instance.onFeat += CheckFeat;
            PlayerManager.Instance.onBattle += CheckBattle;

            ProductionManager.Instance.onTrackAdd += CheckTrackChartPosition;
            ProductionManager.Instance.onAlbumAdd += CheckAlbumChartPosition;
            ProductionManager.Instance.onClipAdd += CheckClipLoser;
            ProductionManager.Instance.onConcertAdd += CheckConcertPlace;
        }
        
        /// <summary>
        /// Листенер события изменения денег
        /// </summary>
        private void CheckMoney(int money)
        {
            BaseCheckValue(AchievementsType.Money, money);
        }

        /// <summary>
        /// Листенер события изменения фанатов
        /// </summary>
        private void CheckFans(int fans)
        {
            BaseCheckValue(AchievementsType.Fans, fans);
        }

        /// <summary>
        /// Листенер события изменения хайпа
        /// </summary>
        private void CheckHype(int hype)
        {
            BaseCheckValue(AchievementsType.HypeBeast, hype);
        }

        /// <summary>
        /// Листенер события выпуска нового трека
        /// </summary>
        private void CheckTrackChartPosition(TrackInfo trackInfo)
        {
            MultipleCheckValue(AchievementsType.TrackChartPosition, trackInfo.ChartPosition,
                info => info.Achievement.CompareValue);
        }

        /// <summary>
        /// Листенер события выпуска нового альбома
        /// </summary>
        private void CheckAlbumChartPosition(AlbumInfo albumInfo)
        {
            MultipleCheckValue(AchievementsType.AlbumChartPosition, albumInfo.ChartPosition,
                info => info.Achievement.CompareValue);
        }

        /// <summary>
        /// Листенер события выпуска нового клипа
        /// </summary>
        private void CheckClipLoser(ClipInfo clipInfo)
        {
            BaseCheckValue(AchievementsType.ClipLoser, clipInfo.Dislikes);
        }

        /// <summary>
        /// Листенер события организации нового концерта (по завершению)
        /// </summary>
        private void CheckConcertPlace(ConcertInfo concertInfo)
        {
            _lastConcertPlaceName = concertInfo.LocationName;
            EqualCheckValue(AchievementsType.ConcertPlace, concertInfo.LocationId, () => { _lastConcertPlaceName = "";});
        }

        /// <summary>
        /// Листенер события фита с каким-то реальным репером
        /// </summary>
        private void CheckFeat(int raperId)
        {
            EqualCheckValue(AchievementsType.Feat, raperId, null);
        }

        /// <summary>
        /// Листенер события баттла с реальным репером
        /// </summary>
        private void CheckBattle(int raperId)
        {
            EqualCheckValue(AchievementsType.Battle, raperId, null);
        }
        
        /// <summary>
        /// Базовая функция проверки выполнения условия достижения.
        /// Не рассматривает возможность получения сразу нескольких достижений одного типа
        /// </summary>
        private void BaseCheckValue(AchievementsType type, int value)
        {
            if(!TryGetInfo(type, out var lockedInfos))
                return;
            
            var achievementInfo = lockedInfos.OrderBy(info => info.Achievement.CompareValue).First();
            if (achievementInfo.CheckCondition(value, achievementInfo.Achievement.CompareValue))
                AddAchievement(achievementInfo.Achievement);
        }

        /// <summary>
        /// Ищет все новые заработанные достижения указанного типа, фиксирует все в PlayerHistory,
        /// а выводит в UI лишь первое, согласно ключу сортировки 
        /// (в текущей реализации, лучший результат, например, из ТопЧарта 1 и 50 выведет "Топ 1")
        /// </summary>
        private void MultipleCheckValue(AchievementsType type, int value, Func<AchievementInfo, int> orderSelector)
        {
            if(!TryGetInfo(type, out var lockedInfos))
                return;

            var achievementInfos = lockedInfos
                .Where(info => info.CheckCondition(value, info.Achievement.CompareValue))
                .ToArray();

            if (achievementInfos.Any())
            {
                var newUnlockedInfos = achievementInfos.OrderBy(orderSelector).ToArray();
                for (var i = 0; i < newUnlockedInfos.Length; i++)
                    AddAchievement(newUnlockedInfos[i].Achievement, i == 0);
            }
        }
        
        /// <summary>
        /// Базовая функция проверки выполнения условия достижения на точное совпадение
        /// </summary>
        private void EqualCheckValue(AchievementsType type, int value, [CanBeNull] Action clearBufferVariable)
        {
            if(!TryGetInfo(type, out var lockedInfos))
                return;

            var achievementInfo =
                lockedInfos.FirstOrDefault(info => info.CheckCondition(value, info.Achievement.CompareValue));
            
            if(achievementInfo != default)
                AddAchievement(achievementInfo.Achievement);

            clearBufferVariable?.Invoke();
        }

        /// <summary>
        /// Поиск достижений определнного типа.
        /// </summary>
        private bool TryGetInfo(AchievementsType type, out IEnumerable<AchievementInfo> lockedInfos)
        {
            lockedInfos = achievementsData.LockedInfos.Where(info => info.Achievement.Type == type);
            return lockedInfos.Any();
        }
        
        /// <summary>
        /// Функция добавления ачивки в список заработанных и вывода в UI
        /// </summary>
        private void AddAchievement(Achievement achievement, bool showUi = true)
        {
            achievement.Unlocked = true;
            PlayerManager.Data.Achievements.Add(achievement);

            if (!showUi)
                return;

            void Notification()
            {
                Debug.LogWarning("Показ ачивки");
                void NotificationClickAction()
                {
                    var compareValueString = GetCompareValueString(achievement);
                    var localizedAchievementName = LocalizationManager.Instance.Get(achievement.Type.GetDescription());
                    var achievementString = $"{localizedAchievementName}: {compareValueString}";
                    //todo: Добавить дескриптион для ачивки
                    var description = "Some description";
            
                    newAchievementsPage.ShowNewAchievement(achievementString, description);
                }
                newAchievementEffect.Show(newAchievementSprite, NotificationClickAction);
            }
            IndependentNotificationManager.Instance.AddNotification(Notification);
        }

        /// <summary>
        /// Получение представления значения для получения достижения
        /// </summary>
        private string GetCompareValueString(Achievement achievement)
        {
            switch (achievement.Type)
            {
                case AchievementsType.ConcertPlace:
                {
                    return _lastConcertPlaceName;
                }
                case AchievementsType.Feat:
                case AchievementsType.Battle:
                {
                    //todo: Добавить поиск репера из списка по аналогии с местом проведеняи концерта
                    return "";
                }
                default:
                {
                    return achievement.CompareValue.GetDisplay();
                }
            }
        }
    }
}