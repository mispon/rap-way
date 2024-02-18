using System;
using Game.Labels;
using Models.Production;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Production.Analyzers
{
    /// <summary>
    /// Анализатор альбома
    /// </summary>
    public class AlbumAnalyzer : Analyzer<AlbumInfo>
    {
        /// <summary>
        /// Анализирует успешность альбома
        /// </summary>
        public override void Analyze(AlbumInfo album)
        {
            GameStatsManager.Analyze(album.TrendInfo);
            
            float qualityPoints = CalculateAlbumQuality(album);
            album.Quality = qualityPoints;
            
            var hitDice = Random.Range(0f, 1f);
            if (qualityPoints >= settings.AlbumHitThreshold || hitDice <= settings.AlbumHitChance) 
            {
                album.IsHit = true;
            }

            int fansAmount = GetFans();
            album.ListenAmount = CalculateListensAmount(
                fansAmount,
                qualityPoints, 
                album.TrendInfo.EqualityValue,
                album.IsHit
            );

            if (qualityPoints >= settings.AlbumChartsThreshold)
            {
                album.ChartPosition = CalculateChartPosition();
            }
            
            album.FansIncome = CalcNewFansCount(fansAmount, qualityPoints);
            album.MoneyIncome = CalcMoneyIncome(album.ListenAmount, settings.AlbumListenCost);
            
            if (LabelsManager.Instance.IsPlayerInGameLabel())
            {
                int labelsFee = album.MoneyIncome / 100 * 20;
                album.MoneyIncome -= labelsFee;
            }
        }

        /// <summary>
        /// Определяет качество альбома в зависимости от очков работы и попадания в тренды
        /// </summary>
        /// <returns>Показатель качества альбома от [base] до 1.0</returns>
        private float CalculateAlbumQuality(AlbumInfo album)
        {
            float qualityPoints = settings.AlbumBaseQuality;

            float workPointsFactor = CalculateWorkPointsFactor(album.TextPoints, album.BitPoints);
            qualityPoints += workPointsFactor;

            // Определяем бонус в качество от попадания в тренды
            
            qualityPoints += album.TrendInfo.EqualityValue;

            return Mathf.Min(qualityPoints, 1f);
        }

        /// <summary>
        /// Фактор рабочих очков - это доля набранных рабочих очков в измерении качества альбома
        /// </summary>
        private float CalculateWorkPointsFactor(int textPoints, int bitPoints)
        {
            float workPointsImpact = 1f - settings.AlbumBaseQuality;
            float workPointsRatio = 1f * (textPoints + bitPoints) / settings.AlbumWorkPointsMax;

            float workPointsFactor = workPointsImpact * Mathf.Min(workPointsRatio, 1f);

            return workPointsFactor;
        }

        /// <summary>
        /// Вычисляет количество прослушиваний на основе качества альбома, кол-ва фанатов и уровня хайпа
        /// </summary>
        private int CalculateListensAmount(
            int fans,
            float quality,
            float trandsMatchFactor, 
            bool isHit    
        )
        {
            // Количество фанатов, ждущих трек, зависит от уровня хайпа
            int activeFans = Convert.ToInt32(fans * (0.5f + GetHypeFactor()));
            
            // Активность прослушиваний трека фанатами зависит от его качества
            const float maxFansActivity = 5f;
            float activity = 1.0f + (maxFansActivity * quality);

            int listens = Convert.ToInt32(Math.Ceiling(activeFans * activity));
            
            // Попадание в тренды так же увеличивает прослушивания
            listens = (int) (listens * (1f + trandsMatchFactor));
            
            if (isHit)
            {
                try
                {
                    listens = checked(listens * 5);
                }
                catch (OverflowException)
                {
                    listens = int.MaxValue;
                }
            }
            
            return AddFuzzing(listens);
        }

        private int CalculateChartPosition()
        {
            if (GetFans() < settings.MinFansForCharts)
            {
                return 0;
            }

            const int maxPosition = 100;
            return Random.Range(1, maxPosition);
        }
    }
}