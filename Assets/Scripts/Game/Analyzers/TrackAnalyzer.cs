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
            // todo: analyze track
            
            track.ListenAmount = Random.Range(1, 100);
            track.ChartPosition = Random.Range(1, 100);
            track.FansIncome = Random.Range(1, 100);
            track.MoneyIncome = Random.Range(1, 100);
        }
    }
}