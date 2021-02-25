using Core;
using Models.Info.Production;
using UnityEngine;

namespace Game.Analyzers
{
    /// <summary>
    /// Анализатор трека
    /// </summary>
    public class TrackAnalyzer : Analyzer<TrackInfo>
    {
        /// <summary>
        /// Анализирует успешность трека
        /// </summary>
        public override void Analyze(TrackInfo track)
        {
            var totalFans = PlayerManager.Data.Fans;
            
            GameStatsManager.Analyze(track.TrendInfo);
            
            var resultPoints = settings.TrackFansToPointsIncomeCurve.Evaluate(totalFans) * (track.TextPoints + track.BitPoints);
            resultPoints += resultPoints * Mathf.Lerp(0, settings.TrackTrendsEqualityMultiplier, track.TrendInfo.EqualityValue);

            track.ListenAmount = (int) settings.TrackListenAmountCurve.Evaluate(resultPoints);
            track.ChartPosition = (int) settings.TrackChartPositionCurve.Evaluate(track.ListenAmount);

            var hypeImpact = settings.TrackHypeImpactMultiplier * PlayerManager.Data.Hype;
            var fansIncomeFromListeners = settings.TrackFansIncomeCurve.Evaluate(totalFans) * track.ListenAmount;
            track.FansIncome = (int) (hypeImpact * fansIncomeFromListeners);
            track.MoneyIncome = settings.TrackMoneyIncomeMultiplier * track.ListenAmount;
        }
    }
}