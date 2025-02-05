using System;
using Game.Player;
using Game.Settings;
using Models.Production;
using UnityEngine;
using Random = UnityEngine.Random;
using PlayerAPI = Game.Player.PlayerPackage;
using LabelsAPI = Game.Labels.LabelsPackage;

namespace Game.Production.Analyzers
{
    public class AlbumAnalyzer : Analyzer
    {
        // TODO: Rewrite and remove all hardcoded deps
        // analyzer must analyze all tracks independently player or AI is creator
        public static void Analyze(AlbumInfo album, GameSettings settings)
        {
            var trendsMatch = GameStatsManager.Analyze(album.TrendInfo);

            var qualityPoints = CalculateAlbumQuality(
                album,
                trendsMatch,
                settings.Album.BaseQuality,
                settings.Album.WorkPointsMax
            );
            album.Quality = qualityPoints;

            var hitDice = Random.Range(0f, 1f);
            if (qualityPoints >= settings.Album.HitThreshold || hitDice <= settings.Album.HitChance)
            {
                album.IsHit = true;
            }

            var fansAmount = GetFans(album.CreatorId, settings.Player.BaseFans);

            album.ListenAmount = CalculateListensAmount(
                album.CreatorId,
                fansAmount,
                qualityPoints,
                trendsMatch,
                album.IsHit
            );

            if (qualityPoints >= settings.Album.ChartsThreshold)
            {
                album.ChartPosition = CalculateChartPosition(
                    album.CreatorId,
                    settings.Player.BaseFans,
                    settings.Player.MinFansForCharts
                );
            }

            album.FansIncome = CalcNewFansCount(
                fansAmount,
                qualityPoints,
                settings.Player.MinFansIncome,
                settings.Player.MaxFansIncome
            );
            album.MoneyIncome = CalcMoneyIncome(
                album.ListenAmount,
                settings.Album.ListenCost,
                settings.Player.MinMoneyIncome,
                settings.Player.MaxMoneyIncome
            );

            if (LabelsAPI.Instance.IsPlayerInGameLabel())
            {
                var labelsFee = album.MoneyIncome / 100 * 20;
                album.MoneyIncome -= labelsFee;
            }
        }

        /// <summary>
        ///     Определяет качество альбома в зависимости от очков работы и попадания в тренды
        /// </summary>
        /// <returns>Показатель качества альбома от [base] до 1.0</returns>
        private static float CalculateAlbumQuality(AlbumInfo album, float trendsMatch, float baseQuality, int maxWorkPoints)
        {
            var qualityPoints = baseQuality;

            var workPointsFactor = CalculateWorkPointsFactor(album.TextPoints, album.BitPoints, baseQuality, maxWorkPoints);
            qualityPoints += workPointsFactor;

            if (IsPlayerCreator(album.CreatorId))
            {
                var goodsPointsFactor = PlayerAPI.Inventory.GetQualityImpact();
                qualityPoints += goodsPointsFactor;    
            }
            
            // Определяем бонус в качество от попадания в тренды
            qualityPoints += trendsMatch;

            return Mathf.Min(qualityPoints, 1f);
        }

        /// <summary>
        ///     Фактор рабочих очков - это доля набранных рабочих очков в измерении качества альбома
        /// </summary>
        private static float CalculateWorkPointsFactor(int textPoints, int bitPoints, float baseQuality, int maxWorkPoints)
        {
            var workPointsImpact = 1f - baseQuality;
            var workPointsRatio  = 1f * (textPoints + bitPoints) / maxWorkPoints;

            var workPointsFactor = workPointsImpact * Mathf.Min(workPointsRatio, 1f);

            return workPointsFactor;
        }

        /// <summary>
        ///     Вычисляет количество прослушиваний на основе качества альбома, кол-ва фанатов и уровня хайпа
        /// </summary>
        private static int CalculateListensAmount(int creatorId, int fans, float quality, float trendsMatchFactor, bool isHit)
        {
            try
            {
                // Количество фанатов, ждущих трек, зависит от уровня хайпа
                var activeFans = Convert.ToInt32(fans * (0.5f + GetHypeFactor(creatorId)));

                // Активность прослушиваний трека фанатами зависит от его качества
                const float maxFansActivity = 5f;
                var         activity        = 1.0f + maxFansActivity * quality;

                var listens = Convert.ToInt32(Math.Ceiling(activeFans * activity));

                // Попадание в тренды так же увеличивает прослушивания
                listens = (int) (listens * (1f + trendsMatchFactor));

                if (isHit)
                {
                    listens *= 5;
                }

                return AddFuzzing(listens);

            } catch (Exception)
            {
                return AddFuzzing(int.MaxValue);
            }
        }

        private static int CalculateChartPosition(int creatorId, int baseFans, int minFansForChart)
        {
            if (GetFans(creatorId, baseFans) < minFansForChart)
            {
                return 0;
            }

            const int maxPosition = 100;
            return Random.Range(1, maxPosition);
        }
    }
}