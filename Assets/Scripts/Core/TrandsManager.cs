using System;
using Core.Interfaces;
using Enums;
using Game;
using Models.Game;
using Utils;

namespace Core
{
    /// <summary>
    /// Класс управления трендами
    /// </summary>
    public class TrandsManager: Singleton<TrandsManager>, IStarter
    {
        private static readonly int STYLESCOUNT = Enum.GetValues(typeof(Styles)).Length;
        private static readonly int THEMESCOUNT = Enum.GetValues(typeof(Themes)).Length;
        
        private Trands Trands => GameManager.Instance.GameStats.Trands;
        private DateTime Now => TimeManager.Instance.Now;
        
        public void OnStart()
        {
            TimeManager.Instance.onWeekLeft += OnCheckTimeToChangeTrands;
        }

        /// <summary>
        /// Получение новой даты обновления трендов
        /// </summary>
        public static DateTime GetNextTimeUpdate(DateTime currentDate)
            => currentDate.AddMonths(UnityEngine.Random.Range(2, 4));

        /// <summary>
        /// Проверяем, наступило ли время для изменения Трендов
        /// </summary>
        private void OnCheckTimeToChangeTrands()
        {
            if (Now < Trands.NextTimeUpdate)
                return;
            
            ChangeTrands(Now);
        }

        /// <summary>
        /// Изменяем тренды
        /// </summary>
        private void ChangeTrands(DateTime now)
        {
            GameManager.Instance.GameStats.Trands = new Trands
            {
                Style = (Styles) UnityEngine.Random.Range(0, STYLESCOUNT),
                Theme = (Themes) UnityEngine.Random.Range(0, THEMESCOUNT),
                NextTimeUpdate = GetNextTimeUpdate(now)
            };
        }
    }
}