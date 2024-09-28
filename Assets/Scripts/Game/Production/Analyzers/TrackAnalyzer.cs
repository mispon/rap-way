using System;
using Models.Production;
using UnityEngine;
using Random = UnityEngine.Random;
using PlayerAPI = Game.Player.PlayerPackage;
using RappersAPI = Game.Rappers.RappersPackage;
using LabelsAPI = Game.Labels.LabelsPackage;

namespace Game.Production.Analyzers
{
    public class TrackAnalyzer : Analyzer<TrackInfo>
    {
        public override void Analyze(TrackInfo track)
        {
            GameStatsManager.Analyze(track.TrendInfo);

            float qualityPoints = CalculateTrackQuality(track);
            track.Quality = qualityPoints;

            if (track.Id != 1)
            {
                var hitDice = Random.Range(0f, 1f);
                if (qualityPoints >= settings.Track.HitThreshold || hitDice <= settings.Track.HitChance)
                {
                    track.IsHit = true;
                }
            }

            int fansAmount = GetFans();
            if (track.FeatId != 0)
            {
                fansAmount += RappersAPI.Instance.GetFansCount(track.FeatId);
            }

            track.ListenAmount = CalculateListensAmount(
                fansAmount,
                qualityPoints,
                track.TrendInfo.EqualityValue,
                track.IsHit
            );

            if (qualityPoints >= settings.Track.ChartsThreshold)
            {
                track.ChartPosition = CalculateChartPosition();
            }

            track.FansIncome = CalcNewFansCount(fansAmount, qualityPoints);
            track.MoneyIncome = CalcMoneyIncome(track.ListenAmount, settings.Track.ListenCost);

            if (LabelsAPI.Instance.IsPlayerInGameLabel() && track.FeatId == 0)
            {
                int labelsFee = track.MoneyIncome / 100 * 20;
                track.MoneyIncome -= labelsFee;
            }

            if (track.Id == 2)
            {
                // fake boost quality and other stuff for tutorial
                track.Quality *= 2;
                track.ListenAmount *= 2;
                track.FansIncome *= 2;
                track.MoneyIncome *= 2;
            }
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

            float goodsPointsFactor = PlayerAPI.Goods.GetQualityImpact();
            qualityPoints += goodsPointsFactor;

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
            var qualityPercent = 1f * workPointsTotal / settings.Track.WorkPointsMax;

            qualityPercent /= 2;

            return Mathf.Min(qualityPercent, 0.5f);
        }

        /// <summary>
        /// Вычисляет количество прослушиваний на основе кол-ва фанатов и уровня хайпа
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
            float activity = 1.0f + maxFansActivity * quality;

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

        private int CalculateChartPosition()
        {
            if (GetFans() <= settings.Player.MinFansForCharts)
            {
                return 0;
            }

            const int maxPosition = 100;
            return Random.Range(1, maxPosition);
        }
    }
}