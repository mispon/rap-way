using Models.Info.Production;
using Core;

namespace Game.Analyzers
{
    /// <summary>
    /// Анализатор концерта
    /// </summary>
    public class ConcertAnalyzer : Analyzer<ConcertInfo>
    {
        /// <summary>
        /// Анализирует успешность концерта
        /// </summary>
        public override void Analyze(ConcertInfo concert)
        {
            var totalFans = PlayerManager.Data.Fans;
            var albumInfo = ProductionManager.GetAlbum(concert.AlbumId);
            var sameConcertsCount = ProductionManager.SameConcertsCount(concert.AlbumId);
            
            var sameConcertImpact = settings.ConcertSamePlaceAgainCurve.Evaluate(sameConcertsCount);
            var albumImpact = settings.ConcertTicketsFromAlbumCurve.Evaluate(albumInfo.ChartPosition);
            var hypeImpact = settings.ConcertHypeImpactMultiplier * PlayerManager.Data.Hype;
            var resultPoints = settings.ConcertFansToPointsIncomeCurve.Evaluate(totalFans) * (concert.ManagementPoints + concert.MarketingPoints);
            
            concert.TicketsSold = (int) settings.ConcertTicketsSoldCurve.Evaluate(sameConcertImpact * albumImpact * hypeImpact * resultPoints);
            
            var fansIncomeFromVisitors = settings.ConcertFansIncomeCurve.Evaluate(totalFans) * concert.TicketsSold;
            concert.FansIncome = (int) fansIncomeFromVisitors;
            concert.MoneyIncome = concert.Income;
        }
    }
}