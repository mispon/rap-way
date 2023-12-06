using System;
using Core;
using Models.Info.Production;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Analyzers
{
    /// <summary>
    /// Анализатор клипа
    /// </summary>
    public class ClipAnalyzer : Analyzer<ClipInfo>
    {
        /// <summary>
        /// Анализирует успешность клипа
        /// </summary>
        public override void Analyze(ClipInfo clip)
        {
            var track = ProductionManager.GetTrack(clip.TrackId);
            
            float qualityPoints = CalculateWorkPointsFactor(clip.DirectorPoints, clip.OperatorPoints);
            clip.Quality = qualityPoints;

            var hitDice = Random.Range(0f, 1f);
            if (qualityPoints >= settings.ClipHitThreshold || hitDice <= settings.ClipHitChance) 
            {
                clip.IsHit = true;
            }

            int fansAmount = GetFans();
            
            clip.Views = CalculateViewsAmount(
                fansAmount, 
                qualityPoints,
                track.ListenAmount, 
                clip.IsHit
            );

            int activeViewers = Convert.ToInt32(clip.Views * settings.ClipActiveViewers);
            var (likes, dislikes) = CalculateReaction(qualityPoints, activeViewers);
            clip.Likes = likes;
            clip.Dislikes = dislikes;

            clip.FansIncome = CalcNewFansCount(fansAmount, qualityPoints);
            clip.MoneyIncome = CalcMoneyIncome(clip.Views, settings.ClipViewCost);
        }

        /// <summary>
        /// Вычисляет вклад рабочих очков в качество трека
        /// </summary>
        private float CalculateWorkPointsFactor(int dirPoints, int opPoints)
        {
            var workPointsTotal = dirPoints + opPoints;
            var qualityPercent = (1f * workPointsTotal) / settings.ClipWorkPointsMax;

            return Mathf.Min(qualityPercent, 1f);
        }

        /// <summary>
        /// Вычисляет количество просмотров на основе качества клипа, кол-ва фанатов и уровня хайпа
        /// </summary>
        private int CalculateViewsAmount(
            int fans,
            float quality, 
            int trackListenAmount,
            bool isHit
        ) {
            // Количество фанатов, ждущих трек, зависит от уровня хайпа
            int activeFans = Convert.ToInt32(fans * (0.5f + GetHypeFactor()));
            
            // Активность прослушиваний трека фанатами зависит от его качества
            const float maxFansActivity = 5f;
            float activity = 1.0f + (maxFansActivity * quality);

            int views = Convert.ToInt32(Math.Ceiling(activeFans * activity));
            
            // Успешность трека увеличивает просмотры
            views += Convert.ToInt32(trackListenAmount * 0.1f);
            
            if (isHit)
            {
                try
                {
                    views = checked(views * 5);
                }
                catch (OverflowException e)
                {
                    views = int.MaxValue;
                }
            }
            
            return AddFuzzing(views);
        }

        /// <summary>
        /// Вычисляет количество лайков / дизлайков
        /// </summary>
        private static (int likes, int dislikes) CalculateReaction(float clipQuality, int activeViewers)
        {
            int likes = Convert.ToInt32(clipQuality * activeViewers);
            int dislikes = Convert.ToInt32((1f - clipQuality) * activeViewers);
            
            return (AddFuzzing(likes), AddFuzzing(dislikes));
        }
    }
}