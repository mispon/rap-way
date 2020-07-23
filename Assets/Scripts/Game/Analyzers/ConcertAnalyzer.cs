using Models.Info.Production;
using System.Linq;
using UnityEngine;

namespace Game.Analyzers
{
    /// <summary>
    /// Анализатор концерта
    /// </summary>
    public class ConcertAnalyzer : Analyzer<ConcertInfo>
    {
        [Header("Кривая ценности одного очка работы от количества фанатов")] 
        [SerializeField] private AnimationCurve fansToPointsIncomeCurve;

        [Header("Настройки продаж билетов")] 
        [SerializeField, Tooltip("Зависимость числа продажи билетов от очков")] 
        private AnimationCurve ticketsSoldCurve;
        [SerializeField, Tooltip("Зависимость коэффициента продаж билетов от кол-во")] 
        private AnimationCurve sameConcertImpact;
        [SerializeField, Tooltip("Коэффициент влияния хайпа на продажу билетов")] 
        private float hypeImpactMultiplier;
        
        [Header("Настройки прироста фанатов")] 
        [SerializeField, Tooltip("Зависимость коэффициента прироста фанатов от текущих фанатов")] 
        private AnimationCurve fansIncomeCurve;
        
        
        /// <summary>
        /// Анализирует успешность концерта
        /// </summary>
        public override void Analyze(ConcertInfo concert)
        {
            var totalFans = PlayerManager.Data.Fans;

            var sameConcertsCount =
                PlayerManager.Data.History.ConcertList.Count(c => c.LocationId == concert.LocationId);
            var sameConcertMultiplier = sameConcertImpact.Evaluate(sameConcertsCount);
            var hypeImpact = hypeImpactMultiplier * PlayerManager.Data.Hype;
            var resultPoints = fansToPointsIncomeCurve.Evaluate(totalFans) * (concert.ManagementPoints + concert.MarketingPoints);
            concert.TicketsSold = (int) ticketsSoldCurve.Evaluate(sameConcertMultiplier * hypeImpact * resultPoints);
            
            var fansIncomeFromVisitors = fansIncomeCurve.Evaluate(totalFans) * concert.TicketsSold; 
            concert.FansIncome = (int) fansIncomeFromVisitors;
            concert.MoneyIncome = concert.Income;
        }
    }
}