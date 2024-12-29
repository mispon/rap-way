using System;
using Game.Settings;
using Models.Production;
using UnityEngine;
using Random = UnityEngine.Random;
using PlayerAPI = Game.Player.PlayerPackage;
using RappersAPI = Game.Rappers.RappersPackage;
using LabelsAPI = Game.Labels.LabelsPackage;

namespace Game.Production.Analyzers
{
    public class TrackAnalyzer : Analyzer
    {
        // TODO: Rewrite and remove all hardcoded deps
        // analyzer must analyze all tracks independently player or AI is creator
        public static void Analyze(TrackInfo track, GameSettings settings)
        {
            var qualityPoints = CalculateTrackQuality(track, settings.Track.WorkPointsMax);
            track.Quality = qualityPoints;

            if (track.Id != 1)
            {
                var hitDice = Random.Range(0f, 1f);
                if (qualityPoints >= settings.Track.HitThreshold || hitDice <= settings.Track.HitChance)
                {
                    track.IsHit = true;
                }
            }

            var fansAmount = GetFans(track.CreatorId, settings.Player.BaseFans);
            if (track.FeatId != -1)
            {
                fansAmount += RappersAPI.Instance.Get(track.FeatId)?.Fans ?? 0;
            }

            var trendsMatch = GameStatsManager.Analyze(track.TrendInfo);

            track.ListenAmount = CalculateListensAmount(
                track.CreatorId,
                fansAmount,
                qualityPoints,
                trendsMatch,
                track.IsHit
            );

            if (qualityPoints >= settings.Track.ChartsThreshold)
            {
                track.ChartPosition = CalculateChartPosition(
                    track.CreatorId,
                    settings.Player.BaseFans,
                    settings.Player.MinFansForCharts
                );
            }

            track.FansIncome = CalcNewFansCount(
                fansAmount,
                qualityPoints,
                settings.Player.MinFansIncome,
                settings.Player.MaxFansIncome
            );
            track.MoneyIncome = CalcMoneyIncome(
                track.ListenAmount,
                settings.Track.ListenCost,
                settings.Player.MinMoneyIncome,
                settings.Player.MaxMoneyIncome
            );

            if (LabelsAPI.Instance.IsPlayerInGameLabel() && track.FeatId == -1)
            {
                var labelsFee = track.MoneyIncome / 100 * 20;
                track.MoneyIncome -= labelsFee;
            }

            if (IsPlayerCreator(track.CreatorId) && track.Id == 2)
            {
                // fake boost quality and other stuff for tutorial
                track.Quality      *= 2;
                track.ListenAmount *= 2;
                track.FansIncome   *= 2;
                track.MoneyIncome  *= 2;
            }
        }

        /// <summary>
        ///     Определяет качество трека в зависимости от очков работы и попадания в тренды
        /// </summary>
        /// <returns>Показатель качества трека от 0.0 до 1.0</returns>
        private static float CalculateTrackQuality(TrackInfo track, int maxWorkPoints)
        {
            float qualityPoints = 0;

            var workPointsFactor = CalculateWorkPointsFactor(track.TextPoints, track.BitPoints, maxWorkPoints);
            qualityPoints += workPointsFactor;

            var goodsPointsFactor = PlayerAPI.Inventory.GetQualityImpact();
            qualityPoints += goodsPointsFactor;

            return Mathf.Min(qualityPoints, 1f);
        }

        /// <summary>
        ///     Фактор рабочих очков - это доля набранных очков от максимального количества
        ///     Фактор максимум может составлять половину качества трека (другую половину составляет техника),
        ///     поэтому при подсчете значение делится еще на 2
        /// </summary>
        private static float CalculateWorkPointsFactor(int textPoints, int bitPoints, int maxWorkPoints)
        {
            var workPointsTotal = textPoints + bitPoints;
            var qualityPercent  = 1f * workPointsTotal / maxWorkPoints;

            qualityPercent /= 2;

            return Mathf.Min(qualityPercent, 0.5f);
        }

        /// <summary>
        ///     Вычисляет количество прослушиваний на основе кол-ва фанатов и уровня хайпа
        /// </summary>
        private static int CalculateListensAmount(int creatorId, int fans, float quality, float trendsMatchFactor, bool isHit)
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
                try
                {
                    listens = checked(listens * 5);
                } catch (OverflowException)
                {
                    listens = int.MaxValue;
                }
            }

            return AddFuzzing(listens);
        }

        private static int CalculateChartPosition(int creatorId, int baseFans, int minFans)
        {
            if (GetFans(creatorId, baseFans) <= minFans)
            {
                return 0;
            }

            const int maxPosition = 100;
            return Random.Range(1, maxPosition);
        }
    }
}