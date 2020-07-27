using Models.Info.Production;
using System.Linq;
using Core;
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
        [SerializeField, Tooltip("Зависимость коэффициента кол-ва продаж билетов от успеха альбома (номер в чарте)")]
        private AnimationCurve ticketsFromAlbumCurve;
        [SerializeField, Tooltip("Зависимость коэффициента продаж билетов от кол-ва проведенных концертов на тот же альбом")] 
        private AnimationCurve sameConcertCurve;
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
            var albumInfo = ProductionManager.GetAlbum(concert.AlbumId);
            var sameConcertsCount = ProductionManager.SameConcertsCount(concert.AlbumId);
            
            var sameConcertImpact = sameConcertCurve.Evaluate(sameConcertsCount);
            var albumImpact = ticketsFromAlbumCurve.Evaluate(albumInfo.ChartPosition);
            var hypeImpact = hypeImpactMultiplier * PlayerManager.Data.Hype;
            var resultPoints = fansToPointsIncomeCurve.Evaluate(totalFans) * (concert.ManagementPoints + concert.MarketingPoints);
            
            concert.TicketsSold = (int) ticketsSoldCurve.Evaluate(sameConcertImpact * albumImpact * hypeImpact * resultPoints);
            
            var fansIncomeFromVisitors = fansIncomeCurve.Evaluate(totalFans) * concert.TicketsSold; 
            concert.FansIncome = (int) fansIncomeFromVisitors;
            concert.MoneyIncome = concert.Income;
        }
    }
}