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
            
            float clipQuality = CalculateWorkPointsFactor(clip.DirectorPoints, clip.OperatorPoints);
            clip.Quality = clipQuality;

            var hitDice = Random.Range(0f, 1f);
            if (clipQuality >= settings.ClipHitThreshold || hitDice <= settings.ClipHitChance) 
            {
                clip.IsHit = true;
            }
            
            float trackListenFactor = GetListenRatio(track.ListenAmount);
            clip.Views = CalculateViewsAmount(clipQuality, trackListenFactor, clip.IsHit);

            int activeViewers = Convert.ToInt32(clip.Views * settings.ClipActiveViewers);
            
            var (likes, dislikes) = CalculateReaction(clipQuality, activeViewers);
            clip.Likes = likes;
            clip.Dislikes = dislikes;

            var (fans, money) = CalculateIncomes(clipQuality, clip.Views, settings.ClipViewCost);
            clip.FansIncome = Convert.ToInt32(fans * settings.ClipActiveViewers);
            clip.MoneyIncome = money;
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
        private int CalculateViewsAmount(float clipQuality, float trackFactor, bool isHit)
        {
            // Количество фанатов, ждущих клип, зависит от уровня хайпа
            int activeFansAmount = (int) (GetFans() * Mathf.Max(0.5f, GetHypeFactor()));

            // Активность прослушиваний трека фанатами зависит от его качества
            const float maxFansActivity = 5f;
            int views = (int) (activeFansAmount * (maxFansActivity * clipQuality));
 
            // Хайп не только влияет на активность фанатов, но и увеличивает просмотров
            views = (int) (views * (1f + GetHypeFactor()));
            
            // Успешность трека увеличивает просмотры
            views = (int) (views * trackFactor);
            
            if (isHit)
            {
                views *= 5;
            }

            int randomizer = Convert.ToInt32(views * TEN_PERCENTS);
            return Random.Range(views - randomizer, views + randomizer);
        }

        /// <summary>
        /// Вычисляет количество лайков / дизлайков
        /// </summary>
        private static (int likes, int dislikes) CalculateReaction(float clipQuality, int activeViewers)
        {
            int likes = Convert.ToInt32(clipQuality * activeViewers);
            var likesFuzz =  Convert.ToInt32(likes * TEN_PERCENTS);
            
            int dislikes = Convert.ToInt32((1f - clipQuality) * activeViewers);
            var dislikesFuzz =  Convert.ToInt32(dislikes * TEN_PERCENTS);
            
            return (
                Random.Range(likes - likesFuzz, likes + likesFuzz), 
                Random.Range(dislikes - dislikesFuzz, dislikes + dislikesFuzz)
            );
        }
    }
}