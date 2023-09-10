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
            GameStatsManager.Analyze(track.TrendInfo);
            
            float qualityPoints = CalculateTrackQuality(track);
            track.Quality = qualityPoints;

            var hitDice = Random.Range(0f, 1f);
            if (qualityPoints >= settings.TrackHitThreshold || hitDice <= settings.TrackHitChance) 
            {
                track.IsHit = true;
            }
            
            int featFansAmount = 0;
            if (track.Feat != null)
            {
                featFansAmount = track.Feat.Fans * 1_000_000;
            }

            track.ListenAmount = CalculateListensAmount(
                qualityPoints,
                featFansAmount, 
                track.TrendInfo.EqualityValue,
                track.IsHit
            );

            if (track.IsHit)
            {
                track.ChartPosition = CalculateChartPosition();
            }

            var (fans, money) = CalculateIncomes(qualityPoints, track.ListenAmount, settings.TrackListenCost);
            track.FansIncome = fans;
            track.MoneyIncome = money;
        }

        /// <summary>
        /// Определяет качество трека в зависимости от очков работы и попадания в тренды
        /// </summary>
        /// <returns>Показатель качества трека от 0.0 до 1.0</returns>
        private float CalculateTrackQuality(TrackInfo track)
        {
            float qualityPoints = 0;
            
            float workPointsFactor = CalculateWorkPointsFactor(track.TextPoints, track.BitPoints);
            qualityPoints += workPointsFactor;

            float equipPointsFactor = GoodsManager.Instance.GetQualityImpact();
            qualityPoints += equipPointsFactor;
            
            return Mathf.Min(qualityPoints, 1f);
        }

        /// <summary>
        /// Фактор рабочих очков - это доля набранных очков от максимального количества
        /// Фактор максимум может составлять половину качества трека (другую половину составляет техника),
        /// поэтому при подсчете значение делится еще на 2
        /// </summary>
        private float CalculateWorkPointsFactor(int textPoints, int bitPoints)
        {
            var workPointsTotal = textPoints + bitPoints;
            var qualityPercent = (1f * workPointsTotal) / settings.TrackWorkPointsMax;

            qualityPercent /= 2;
            
            return Mathf.Min(qualityPercent, 0.5f);
        }

        /// <summary>
        /// Вычисляет количество прослушиваний на основе кол-ва фанатов и уровня хайпа
        /// </summary>
        private int CalculateListensAmount(
            float trackQuality,
            int featFansAmount,
            float trandsMatchFactor, 
            bool isHit
        ) {
            int totalFansAmount = GetFans() + featFansAmount;
            
            // Количество фанатов, ждущих трек, зависит от уровня хайпа
            int activeFansAmount = (int) (totalFansAmount * Mathf.Max(0.5f, GetHypeFactor()));
            
            // Активность прослушиваний трека фанатами зависит от его качества
            const float maxFansActivity = 5f;
            var listens = (int) (activeFansAmount * (maxFansActivity * trackQuality));

            // Хайп не только влияет на активность фанатов, но и увеличивает прослушивания
            listens = (int) (listens * (1f + GetHypeFactor()));
            
            // Попадание в тренды так же увеличивает прослушивания
            listens = (int) (listens * (1f + trandsMatchFactor));
            
            if (isHit)
            {
                listens *= 5;
            }

            int randomizer = Convert.ToInt32(listens * TEN_PERCENTS);
            return Random.Range(listens - randomizer, listens + randomizer);
        }
        
        private int CalculateChartPosition()
        {
            if (GetFans() <= settings.MinFansForCharts)
            {
                return 0;
            }

            const int maxPosition = 100;
            return Random.Range(1, maxPosition);
        }
    }
}