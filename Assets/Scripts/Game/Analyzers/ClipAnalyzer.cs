using Core;
using Models.Info.Production;
using UnityEngine;

namespace Game.Analyzers
{
    /// <summary>
    /// Анализатор клипа
    /// </summary>
    public class ClipAnalyzer : Analyzer<ClipInfo>
    {
        /// <summary>
        /// Анализирует успешность клипа
        /// </summary>
        public override void Analyze(ClipInfo clip)
        {
            var totalFans = PlayerManager.Data.Fans;
            var trackInfo = ProductionManager.GetTrack(clip.TrackId);
            
            var trackImpact = settings.ClipViewsFromTrackCurve.Evaluate(trackInfo.ChartPosition);
            var resultPoints = settings.ClipFansToPointsIncomeCurve.Evaluate(totalFans) * (clip.OperatorPoints + clip.DirectorPoints);
            
            clip.Views = (int) settings.ClipViewsCurve.Evaluate(resultPoints * trackImpact);
            var marks = clip.Views * Random.Range(settings.ClipMinMarksRatio, settings.ClipMaxMarksRatio);
            clip.Likes = (int) (marks * settings.ClipLikesFromTrackCurve.Evaluate(trackImpact));
            clip.Dislikes = (int) (marks - clip.Likes);

            var hypeImpact = settings.ClipHypeImpactMultiplier * PlayerManager.Data.Hype;
            var fansIncomeFromViews = settings.ClipFansIncomeCurve.Evaluate(totalFans) * clip.Views;
            clip.FansIncome = (int) (hypeImpact * fansIncomeFromViews);
            clip.MoneyIncome = settings.ClipMoneyIncomeMultiplier * clip.Views;
        }
    }
}