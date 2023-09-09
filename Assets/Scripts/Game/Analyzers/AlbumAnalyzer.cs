using System;
using Core;
using Models.Info.Production;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Analyzers
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

            album.ListenAmount = CalculateListensAmount(
                qualityPoints, 
                GetFans(),
                album.TrendInfo.EqualityValue,
                album.IsHit
            );

            if (album.IsHit)
            {
                album.ChartPosition = CalculateChartPosition();
            }
            
            var (fans, money) = CalculateIncomes(qualityPoints, album.ListenAmount, settings.AlbumListenCost);
            album.FansIncome = fans;
            album.MoneyIncome = money;
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
            float albumQuality,
            int fansAmount,
            float trandsMatchFactor, 
            bool isHit    
        )
        {
            // Количество фанатов, ждущих альбом, зависит от уровня хайпа
            int activeFansAmount = (int) (fansAmount * Mathf.Max(0.5f, GetHypeFactor()));
            
            // Активность прослушиваний трека фанатами зависит от его качества
            const float maxFansActivity = 5f;
            var listens = (int) (activeFansAmount * (maxFansActivity * albumQuality));

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
            if (GetFans() < settings.MinFansForCharts)
            {
                return 0;
            }

            const int maxPosition = 100;
            return Random.Range(1, maxPosition);
        }
    }
}