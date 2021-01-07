using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Game;
using Models.Player;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "AchievementData", menuName = "Data/Achievement")]
    public class AchievementsData: ScriptableObject
    {
        [ArrayElementTitle(new []{"Achievement.Type", "Achievement.CompareValue" })]
        public AchievementInfo[] Infos;

        public IEnumerable<AchievementInfo> LockedInfos => Infos.Where(info => !info.Achievement.Unlocked);
        
        /// <summary>
        /// Обновляем данные об уже заработанных достижениях.
        /// Устанавливаем функции проверки получения достижений.
        /// </summary>
        public void Initialize()
        {
            foreach (var info in Infos)
                info.Achievement.Unlocked = PlayerManager.Data.Achievements.Any(ach => ach == info.Achievement);

            var lockedInfos = LockedInfos;

            lockedInfos.SetCondition(AchievementsType.Fans, OverflowConditionFunction);
            lockedInfos.SetCondition(AchievementsType.Money, OverflowConditionFunction);
            lockedInfos.SetCondition(AchievementsType.HypeBeast, OverflowConditionFunction);
            lockedInfos.SetCondition(AchievementsType.ClipLoser, OverflowConditionFunction);

            lockedInfos.SetCondition(AchievementsType.TrackChartPosition, ChartPositionConditionFunction);
            lockedInfos.SetCondition(AchievementsType.AlbumChartPosition, ChartPositionConditionFunction);

            lockedInfos.SetCondition(AchievementsType.ConcertPlace, EqualConditionFunction);
            lockedInfos.SetCondition(AchievementsType.Feat, EqualConditionFunction);
            lockedInfos.SetCondition(AchievementsType.Battle, EqualConditionFunction);
        }

        /// <summary>
        /// Проверка достижения порога
        /// </summary>
        private static bool OverflowConditionFunction(int inputValue, int achievementValue)
            => inputValue >= achievementValue;
   
        /// <summary>
        /// Проверка попадания ниже границы
        /// </summary>
        private static bool ChartPositionConditionFunction(int inputValue, int achievementValue)
            => inputValue <= achievementValue;
        
        /// <summary>
        /// Проверка на соответсвие.
        /// В случае Концерта, Фита и Баттла передаем индекс Площадки/Репера в каком-либо перечислении
        /// </summary>
        private static bool EqualConditionFunction(int inputValue, int achievementValue)
            => achievementValue == inputValue;
    }

    /// <summary>
    /// Информация о достижении: экземпляр достижения и условие получения
    /// </summary>
    [Serializable]
    public class AchievementInfo
    {
        /// <summary>
        /// Экземляр достижения
        /// </summary>
        public Achievement Achievement;
        
        /// <summary>
        /// Функция проверки
        /// </summary>
        public Func<int, int, bool> CheckCondition;
    }

    public static partial class Extensions
    {
        public static void SetCondition(this IEnumerable<AchievementInfo> infos, AchievementsType type,
            Func<int, int, bool> conditionFunc)
        {
            var typedInfos = infos.Where(info => info.Achievement.Type == type);
            foreach (var info in typedInfos)
                info.CheckCondition = conditionFunc;
        }
    }
}