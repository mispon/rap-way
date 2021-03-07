using System;
using Core;
using Models.Info.Production;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Analyzers
{
    /// <summary>
    /// Анализатор трека
    /// </summary>
    public class TrackAnalyzer : Analyzer<TrackInfo>
    {
        /// <summary>
        /// Анализирует успешность трека
        /// </summary>
        public override void Analyze(TrackInfo track)
        {
            float qualityPoints = CalculateTrackQuality(track);

            track.ListenAmount = CalculateListensAmount(qualityPoints, GetFans());
            track.ChartPosition = CalculateChartPosition(track.ListenAmount);

            var (fans, money) = CalculateIncomes(qualityPoints, track.ListenAmount);
            track.FansIncome = fans;
            track.MoneyIncome = money;
        }

        /// <summary>
        /// Определяет качество трека в зависимости от очков работы и попадания в тренды
        /// </summary>
        /// <returns>Показатель качества трека от [base] до 1.0</returns>
        private float CalculateTrackQuality(TrackInfo track)
        {
            float qualityPoints = settings.TrackBaseQuality;

            float workPointsFactor = CalculateWorkPointsFactor(track.TextPoints, track.BitPoints);
            qualityPoints += workPointsFactor;

            // Определяем бонус в качество от попадания в тренды
            GameStatsManager.Analyze(track.TrendInfo);
            qualityPoints += track.TrendInfo.EqualityValue;

            return Mathf.Min(qualityPoints, 1f);
        }

        /// <summary>
        /// Фактор рабочих очков - это доля набранных рабочих очков в измерении качества трека
        /// Пример расчёта, при базовом качестве [0.3] и кол-ве рабочих очков [130 из 250]:
        /// определяем долю качества рабочих очков: 1.0 - 0.3 = 0.7
        /// определяем 1% от доли игрока в масштабе макс. кол-ва очков: 0.7 * (1.0 / 250) = 0.0028
        /// считаем суммарный фактор: 130 * 0.0028 = 0.364 - влад в качество от очков работы
        /// </summary>
        private float CalculateWorkPointsFactor(int textPoints, int bitPoints)
        {
            float workPointsImpact = 1f - settings.TrackBaseQuality;
            float workPointsRatio = 1f * (textPoints + bitPoints) / settings.TrackWorkPointsMax;

            float workPointsFactor = workPointsImpact * Mathf.Min(workPointsRatio, 1f);

            return workPointsFactor;
        }

        /// <summary>
        /// Вычисляет количество прослушиваний на основе качества трека, кол-ва фанатов и уровня хайпа
        /// </summary>
        private int CalculateListensAmount(float trackQuality, int fansAmount)
        {
            bool isHit = Random.Range(0f, 1f) <= settings.TrackHitChance;

            float trackGrade = settings.TrackGradeCurve.Evaluate(trackQuality);
            float hypeFactor = CalculateHypeFactor();

            int listens = Convert.ToInt32(fansAmount * (trackGrade + hypeFactor));
            if (isHit)
            {
                listens *= 2;
            }

            return listens;
        }

        /// <summary>
        /// Фактор хайпа - это доля от общего числа фанов, которая послучает трек
        /// Пример расчёта, при базовом значении хайпа [0.2] и уровне хайпа игрока [55 из 100]:
        /// определяем долю хайпа игрока: 1.0 - 0.2 = 0.8
        /// определяем 1% от доли игрока: 0.8 * 0.01 = 0.008
        /// считаем суммарный фактор: 0.2 + (55 * 0.008) = 0.2 + 0.44 = 0.64
        /// получаем 0.64 - такая доля от общего кол-ва фанатов послушает трек
        /// </summary>
        private float CalculateHypeFactor()
        {
            float playerHypePercent = (1f - settings.TrackBaseHype) * ONE_PERCENT;
            float hypeFactor = settings.TrackBaseHype + PlayerManager.Data.Hype * playerHypePercent;
            return hypeFactor;
        }

        /// <summary>
        /// Позиция в чарте - отношение прослушиваний к общей сумме фанов
        /// </summary>
        private int CalculateChartPosition(int listensAmount)
        {
            float listenRatio = GetListenRatio(listensAmount);

            if (listenRatio <= 0.5f || GetFans() <= settings.MinFansForCharts)
            {
                // Трек послушало меньше половины от общего кол-ва фанатов
                // или слишком мало фанатов
                return 0;
            }

            const int MAX_POSITION = 100;
            float coef = settings.TrackChartCurve.Evaluate(listenRatio);

            int position = (int) Math.Round(MAX_POSITION * coef);
            return position;
        }

        /// <summary>
        /// Вычисляет прибыльность трека
        /// </summary>
        private (int fans, int money) CalculateIncomes(float trackQuality, int listensAmount)
        {
            // Прирост фанатов - количество прослушиваний * коэф. прироста
            var fans = listensAmount * settings.TrackFansIncomeCurve.Evaluate(trackQuality);
            // Доход - количество прослушиваний * стоимость одного прослушивания
            var money = listensAmount * settings.TrackListenCost;

            return (Convert.ToInt32(fans), Convert.ToInt32(money));
        }
    }
}