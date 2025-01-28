using System;
using Game.Settings;
using Models.Production;
using UnityEngine;
using Random = UnityEngine.Random;
using RappersAPI = Game.Rappers.RappersPackage;
using LabelsAPI = Game.Labels.LabelsPackage;

namespace Game.Production.Analyzers
{
    public class ClipAnalyzer : Analyzer
    {
        public static void Analyze(ClipInfo clip, GameSettings settings)
        {
            var track = IsPlayerCreator(clip.CreatorId)
                ? ProductionManager.GetTrack(clip.TrackId)
                : RappersAPI.Instance.GetTrack(clip.CreatorId, clip.TrackId);

            var qualityPoints = CalculateWorkPointsFactor(clip.DirectorPoints, clip.OperatorPoints, settings.Clip.WorkPointsMax);
            clip.Quality = qualityPoints;

            var hitDice = Random.Range(0f, 1f);
            if (qualityPoints >= settings.Clip.HitThreshold || hitDice <= settings.Clip.HitChance)
            {
                clip.IsHit = true;
            }

            var fansAmount = GetFans(clip.CreatorId, settings.Player.BaseFans);

            clip.Views = CalculateViewsAmount(
                clip.CreatorId,
                fansAmount,
                qualityPoints,
                track.ListenAmount,
                clip.IsHit
            );

            var activeViewers = Convert.ToInt32(clip.Views * settings.Clip.ActiveViewers);
            var (likes, dislikes) = CalculateReaction(qualityPoints, activeViewers);

            clip.Likes    = likes;
            clip.Dislikes = dislikes;

            clip.FansIncome = CalcNewFansCount(
                fansAmount,
                qualityPoints,
                settings.Player.MinFansIncome,
                settings.Player.MaxFansIncome
            );
            clip.MoneyIncome = CalcMoneyIncome(
                clip.Views,
                settings.Clip.ViewCost,
                settings.Player.MinMoneyIncome,
                settings.Player.MaxMoneyIncome
            );

            if (LabelsAPI.Instance.IsPlayerInGameLabel())
            {
                var labelsFee = clip.MoneyIncome / 100 * settings.Labels.Fee;
                clip.MoneyIncome -= labelsFee;
            }
        }

        /// <summary>
        ///     Вычисляет вклад рабочих очков в качество трека
        /// </summary>
        private static float CalculateWorkPointsFactor(int dirPoints, int opPoints, int maxWorkPoints)
        {
            var workPointsTotal = dirPoints + opPoints;
            var qualityPercent  = 1f * workPointsTotal / maxWorkPoints;

            return Mathf.Min(qualityPercent, 1f);
        }

        /// <summary>
        ///     Вычисляет количество просмотров на основе качества клипа, кол-ва фанатов и уровня хайпа
        /// </summary>
        private static int CalculateViewsAmount(int creatorId, int fans, float quality, int trackListenAmount, bool isHit)
        {
            try
            {
                // Количество фанатов, ждущих трек, зависит от уровня хайпа
                var activeFans = Convert.ToInt32(fans * (0.5f + GetHypeFactor(creatorId)));

                // Активность прослушиваний трека фанатами зависит от его качества
                const float maxFansActivity = 5f;
                var         activity        = 1.0f + maxFansActivity * quality;

                var views = Convert.ToInt32(Math.Ceiling(activeFans * activity));

                // Успешность трека увеличивает просмотры
                views += Convert.ToInt32(trackListenAmount * 0.1f);

                if (isHit)
                {
                    views *= 5;
                }

                return AddFuzzing(views);

            } catch (Exception)
            {
                return AddFuzzing(int.MaxValue);
            }
        }

        /// <summary>
        ///     Вычисляет количество лайков / дизлайков
        /// </summary>
        private static (int likes, int dislikes) CalculateReaction(float clipQuality, int activeViewers)
        {
            var likes    = Convert.ToInt32(clipQuality * activeViewers);
            var dislikes = Convert.ToInt32((1f - clipQuality) * activeViewers);

            return (AddFuzzing(likes), AddFuzzing(dislikes));
        }
    }
}