using System.Linq;
using Data;
using UnityEngine;
using Models.Info;

namespace Game.Analyzers
{
    public class SocialAnalyzer: Analyzer<SocialInfo>
    {
        [Header("Коэффициент от кол-ва фанатов")]
        [SerializeField, Tooltip("Чем больше фанатов, тем больше за тобой следят, тем больше прирост хайпа")] 
        private float fansMultiplier;

        [Header("Зависимости благотворительности")] 
        [SerializeField, Tooltip("Зависимость коэффициента от доли потраченных денег")]
        private AnimationCurve charityMoneyRatioCurve;
        
        [Header("Данные")] 
        [SerializeField, Tooltip("Базовый коэффициента импакта хайпа по каждому из соц.действий")] 
        private SocialHypeImpactData hypeImpactData;
        
        /// <summary>
        /// Анализирует успешность социального действия
        /// </summary>
        public override void Analyze(SocialInfo social)
        {
            var hypeMultiplier = hypeImpactData.hypeMultipliers.First(el => el.Type == social.Data.Type);
            var resultPoints =  social.PlayerPoints + social.PrManPoints;
            var charityMoneyRatio = social.CharityMoney / (float) PlayerManager.Data.Money;
            var charityMoneyImpact = charityMoneyRatioCurve.Evaluate(charityMoneyRatio);
            
            var hypeIncome = fansMultiplier * hypeMultiplier.Value * charityMoneyImpact * resultPoints;

            social.HypeIncome = (int) Mathf.Clamp(hypeIncome, 0, 100 - PlayerManager.Data.Hype);
        }
    }
}