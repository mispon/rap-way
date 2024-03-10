using System;
using Models.Production;
using UnityEngine;
using PlayerAPI = Game.Player.PlayerPackage;

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

            float activeFans = GetFans() * settings.Socials.ActiveFansGroup;

            social.HypeIncome = Convert.ToInt32(quality * 100);
            social.Likes = Convert.ToInt32(activeFans * quality);
        }

        /// <summary>
        /// Вычисляет качество для пожертвования
        /// </summary>
        private float CalculateCharityQuality(int workPoints, int charityAmount)
        {
            float maxCharity = PlayerAPI.Data.Money / 10f;
            float charityImpact = settings.Socials.CharitySizeImpact * (charityAmount / maxCharity);

            float workPointsImpact = 1f - settings.Socials.CharitySizeImpact;
            float workPointsRatio = 1f * workPoints / settings.Socials.WorkPointsMax;
            float workImpact = workPointsImpact * Mathf.Min(workPointsRatio, 1f);

            return charityImpact + workImpact;
        }

        /// <summary>
        /// Вычисляет качество для всех остальных соц. действий
        /// </summary>
        private float GetAllOtherQuality(int workPoints)
        {
            return 1f * Mathf.Min(workPoints, settings.Socials.WorkPointsMax) / settings.Socials.WorkPointsMax;
        }
    }
}