using Models.Production;
using UnityEngine;

namespace Analyzers
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
            // todo: analyze track
            
            track.ListensCount = Random.Range(1, 100);
            track.ChartPosition = 0;
            track.FansIncome = Random.Range(1, 100);
            track.MoneyIncome = Random.Range(1, 100);
        }
    }
}