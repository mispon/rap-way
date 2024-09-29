using System;
using Models.Production;
using UnityEngine;
using Random = UnityEngine.Random;
using LabelsAPI = Game.Labels.LabelsPackage;

namespace Game.Production.Analyzers
{
    public class AlbumAnalyzer : Analyzer<AlbumInfo>
    {
        // TODO: Rewrite and remove all hardcoded deps
        // analyzer must analyze all tracks independently player or AI is creator
        public override void Analyze(AlbumInfo album)
        {
            float trendsMatch = GameStatsManager.Analyze(album.TrendInfo);

            float qualityPoints = CalculateAlbumQuality(album.TextPoints, album.BitPoints, trendsMatch);
            album.Quality = qualityPoints;

            var hitDice = Random.Range(0f, 1f);
            if (qualityPoints >= settings.Album.HitThreshold || hitDice <= settings.Album.HitChance)
            {
                album.IsHit = true;
            }

            int fansAmount = GetFans(album.CreatorId);

            album.ListenAmount = CalculateListensAmount(
                album.CreatorId,
                fansAmount,
                qualityPoints,
                trendsMatch,
                album.IsHit
            );

            if (qualityPoints >= settings.Album.ChartsThreshold)
            {
                album.ChartPosition = CalculateChartPosition(album.CreatorId);
            }

            album.FansIncome = CalcNewFansCount(fansAmount, qualityPoints);
            album.MoneyIncome = CalcMoneyIncome(album.ListenAmount, settings.Album.ListenCost);

            if (LabelsAPI.Instance.IsPlayerInGameLabel())
            {
                int labelsFee = album.MoneyIncome / 100 * 20;
                album.MoneyIncome -= labelsFee;
            }
        }

        /// <summary>
        /// Определяет качество альбома в зависимости от очков работы и попадания в тренды
        /// </summary>
        /// <returns>Показатель качества альбома от [base] до 1.0</returns>
        private float CalculateAlbumQuality(int textPoints, int bitPoints, float trendsMatch)
        {
            float qualityPoints = settings.Album.BaseQuality;

            float workPointsFactor = CalculateWorkPointsFactor(textPoints, bitPoints);
            qualityPoints += workPointsFactor;

            // Определяем бонус в качество от попадания в тренды
            qualityPoints += trendsMatch;

            return Mathf.Min(qualityPoints, 1f);
        }

        /// <summary>
        /// Фактор рабочих очков - это доля набранных рабочих очков в измерении качества альбома
        /// </summary>
        private float CalculateWorkPointsFactor(int textPoints, int bitPoints)
        {
            float workPointsImpact = 1f - settings.Album.BaseQuality;
            float workPointsRatio = 1f * (textPoints + bitPoints) / settings.Album.WorkPointsMax;

            float workPointsFactor = workPointsImpact * Mathf.Min(workPointsRatio, 1f);

            return workPointsFactor;
        }

        /// <summary>
        /// Вычисляет количество прослушиваний на основе качества альбома, кол-ва фанатов и уровня хайпа
        /// </summary>
        private int CalculateListensAmount(
            int creatorId,
            int fans,
            float quality,
            float trandsMatchFactor,
            bool isHit
        )
        {
            // Количество фанатов, ждущих трек, зависит от уровня хайпа
            int activeFans = Convert.ToInt32(fans * (0.5f + GetHypeFactor(creatorId)));

            // Активность прослушиваний трека фанатами зависит от его качества
            const float maxFansActivity = 5f;
            float activity = 1.0f + (maxFansActivity * quality);

            int listens = Convert.ToInt32(Math.Ceiling(activeFans * activity));

            // Попадание в тренды так же увеличивает прослушивания
            listens = (int)(listens * (1f + trandsMatchFactor));

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

        private int CalculateChartPosition(int creatorId)
        {
            if (GetFans(creatorId) < settings.Player.MinFansForCharts)
            {
                return 0;
            }

            const int maxPosition = 100;
            return Random.Range(1, maxPosition);
        }
    }
}