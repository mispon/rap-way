using System;
using Game.Settings;
using Models.Production;
using UnityEngine;
using PlayerAPI = Game.Player.PlayerPackage;

namespace Game.Production.Analyzers
{
    public class SocialAnalyzer : Analyzer
    {
        public static void Analyze(SocialInfo social, GameSettings settings)
        {
            var quality = social.CharityAmount > 0
                ? CalculateCharityQuality(
                    social.WorkPoints,
                    social.CharityAmount,
                    settings.Socials.CharitySizeImpact,
                    settings.Socials.WorkPointsMax
                )
                : GetAllOtherQuality(social.WorkPoints, settings.Socials.WorkPointsMax);

            const int playerId   = -1;
            var       activeFans = GetFans(playerId, settings.Player.BaseFans) * settings.Socials.ActiveFansGroup;

            social.HypeIncome = Convert.ToInt32(quality * 100);
            social.Likes      = Convert.ToInt32(activeFans * quality);
        }

        private static float CalculateCharityQuality(int workPoints, int charityAmount, float charitySizeImpact, int maxWorkPoints)
        {
            var maxCharity    = PlayerAPI.Data.Money / 10f;
            var charityImpact = charitySizeImpact * (charityAmount / maxCharity);

            var workPointsImpact = 1f - charitySizeImpact;
            var workPointsRatio  = 1f * workPoints / maxWorkPoints;
            var workImpact       = workPointsImpact * Mathf.Min(workPointsRatio, 1f);

            return charityImpact + workImpact;
        }

        private static float GetAllOtherQuality(int workPoints, int maxWorkPoints)
        {
            return 1f * Mathf.Min(workPoints, maxWorkPoints) / maxWorkPoints;
        }
    }
}