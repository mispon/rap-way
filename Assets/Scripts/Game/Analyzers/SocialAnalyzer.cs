using Enums;
using UnityEngine;
using Models.Info;

namespace Game.Analyzers
{
    public class SocialAnalyzer: Analyzer<SocialInfo>
    {
        /// <summary>
        /// Анализирует успешность социального действия
        /// </summary>
        public override void Analyze(SocialInfo social)
        {
            var hypeMultiplier = settings.SocialsHypeImpactData[(int) social.Type];
            
            var hypeIncome = settings.SocialsFansMultiplier * hypeMultiplier * social.WorkPoints;
            if (social.Type == SocialType.Charity)
            {
                var charityMoneyRatio = social.CharityAmount / (float) PlayerManager.Data.Money;
                var charityMoneyImpact = settings.SocialsCharityMoneyRatioCurve.Evaluate(charityMoneyRatio);
                
                hypeIncome *= charityMoneyImpact;
            }

            social.HypeIncome = (int) Mathf.Clamp(hypeIncome, 0, 100 - PlayerManager.Data.Hype);
        }
    }
}