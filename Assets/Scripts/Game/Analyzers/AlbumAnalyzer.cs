using Core;
using Models.Info.Production;
using UnityEngine;

namespace Game.Analyzers
{
    /// <summary>
    /// Анализатор альбома
    /// </summary>
    public class AlbumAnalyzer : Analyzer<AlbumInfo>
    {
        /// <summary>
        /// Анализирует успешность альбома 
        /// </summary>
        public override void Analyze(AlbumInfo album)
        {
            var totalFans = PlayerManager.Data.Fans;
            
            GameStatsManager.Analyze(album.TrendInfo);
            
            var resultPoints = settings.AlbumFansToPointsIncomeCurve.Evaluate(totalFans) * (album.TextPoints + album.BitPoints);
            resultPoints += resultPoints * Mathf.Lerp(0, settings.AlbumTrendsEqualityMultiplier, album.TrendInfo.EqualityValue);

            album.ListenAmount = (int) settings.AlbumListenAmountCurve.Evaluate(resultPoints);
            album.ChartPosition = (int) settings.AlbumChartPositionCurve.Evaluate(album.ListenAmount);

            var hypeImpact = settings.AlbumHypeImpactMultiplier * PlayerManager.Data.Hype;
            var fansIncomeFromListeners = settings.AlbumFansIncomeCurve.Evaluate(totalFans) * album.ListenAmount;
            album.FansIncome = (int) (hypeImpact * fansIncomeFromListeners);
            album.MoneyIncome = settings.AlbumMoneyIncomeMultiplier * album.ListenAmount;
        }
    }
}