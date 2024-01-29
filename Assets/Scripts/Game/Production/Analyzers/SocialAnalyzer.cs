using System;
using Game.Player;
using Models.Production;
using UnityEngine;

namespace Game.Production.Analyzers
{
    /// <summary>
    /// Анализатор соц. действий
    /// </summary>
    public class SocialAnalyzer: Analyzer<SocialInfo>
    {
        /// <summary>
        /// Анализирует успешность социального действия
        /// </summary>
        public override void Analyze(SocialInfo social)
        {
            float quality = social.CharityAmount > 0
                ? CalculateCharityQuality(social.WorkPoints, social.CharityAmount)
                : GetAllOtherQuality(social.WorkPoints);

            float activeFans = GetFans() * settings.SocialsActiveFansGroup;

            social.HypeIncome = Convert.ToInt32(quality * 100);
            social.Likes = Convert.ToInt32(activeFans * quality);
        }

        /// <summary>
        /// Вычисляет качество для пожертвования
        /// </summary>
        private float CalculateCharityQuality(int workPoints, int charityAmount)
        {
            float maxCharity = PlayerManager.Data.Money / 10f;
            float charityImpact = settings.SocialsCharitySizeImpact * (charityAmount / maxCharity);

            float workPointsImpact = 1f - settings.SocialsCharitySizeImpact;
            float workPointsRatio = 1f * workPoints / settings.SocialsWorkPointsMax;
            float workImpact = workPointsImpact * Mathf.Min(workPointsRatio, 1f);

            return charityImpact + workImpact;
        }

        /// <summary>
        /// Вычисляет качество для всех остальных соц. действий
        /// </summary>
        private float GetAllOtherQuality(int workPoints)
        {
            return 1f * Mathf.Min(workPoints, settings.SocialsWorkPointsMax) / settings.SocialsWorkPointsMax;
        }
    }
}