using System;
using System.Linq;
using Core;
using Core.Localization;
using Core.OrderedStarter;
using Enums;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.Game;
using MessageBroker.Messages.Time;
using Models.Trends;
using ScriptableObjects;
using UniRx;
using UnityEngine;

// ReSharper disable CoVariantArrayConversion

namespace Game
{
    /// <summary>
    /// Класс управления трендами
    /// </summary>
    public class GameStatsManager : Singleton<GameStatsManager>, IStarter
    {
        private static readonly int STYLES_COUNT = Enum.GetValues(typeof(Styles)).Length;
        private static readonly int THEMES_COUNT = Enum.GetValues(typeof(Themes)).Length;

        [Header("Данные сравнения")]
        [SerializeField] private TrendsCompareData trendsCompareData;

        private readonly CompositeDisposable _disposable = new();

        public void OnStart()
        {
            MsgBroker.Instance
                .Receive<DayLeftMessage>()
                .Subscribe(e => OnDayLeft())
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<WeekLeftMessage>()
                .Subscribe(e => OnWeekLeft())
                .AddTo(_disposable);
        }

        /// <summary>
        /// Получение новой даты обновления трендов
        /// </summary>
        public static DateTime GetNextTimeUpdate(DateTime currentDate)
        {
            return currentDate.AddMonths(UnityEngine.Random.Range(2, 4));
        }

        /// <summary>
        /// Истечение игрового дня 
        /// </summary>
        private static void OnDayLeft()
        {
            var data = GameManager.Instance.GameStats;

            data.SocialsCooldown = Math.Max(0, data.SocialsCooldown - 1);
            data.ConcertCooldown = Math.Max(0, data.ConcertCooldown - 1);
        }

        /// <summary>
        /// Проверяем, наступило ли время для изменения Трендов
        /// </summary>
        private static void OnWeekLeft()
        {
            var now = TimeManager.Instance.Now;

            if (now < GameManager.Instance.GameStats.Trends.NextTimeUpdate)
                return;

            ChangeTrends(now);
        }

        private static void ChangeTrends(DateTime now)
        {
            GameManager.Instance.GameStats.Trends = new Trends
            {
                Style = (Styles)UnityEngine.Random.Range(0, STYLES_COUNT),
                Theme = (Themes)UnityEngine.Random.Range(0, THEMES_COUNT),
                NextTimeUpdate = GetNextTimeUpdate(now)
            };
        }

        public static float Analyze(TrendInfo info)
        {
            var currentTrend = GameManager.Instance.GameStats.Trends;
            var compareData = Instance.trendsCompareData;

            var styleEquality = compareData.StylesCompareInfos.AnalyzeEquality(currentTrend.Style, info.Style);
            var themeEquality = compareData.ThemesCompareInfos.AnalyzeEquality(currentTrend.Theme, info.Theme);

            return styleEquality + themeEquality;
        }

        private void OnDestroy()
        {
            _disposable.Clear();
        }
    }

    public static class Extension
    {
        public static float AnalyzeEquality<T>(this BaseCompareInfo<T>[] array, T currentValue, T selectedValue)
        {
            if (currentValue.Equals(selectedValue))
                return 0.1f;

            var equalInfos = array.Where(el => el.IsEqualTo(currentValue)).ToArray();
            var equalInfosCount = equalInfos.Length;

            if (equalInfosCount == 0 || !equalInfos.Any(el => el.IsEqualTo(selectedValue)))
                return 0;

            return 0.1f / (equalInfosCount + 1);
        }
    }
}