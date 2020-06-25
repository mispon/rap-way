using Models.Production;
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
            // todo: analyze album
            
            album.ListenAmount = Random.Range(1, 100);
            album.ChartPosition = Random.Range(1, 100);
            album.FansIncome = Random.Range(1, 100);
            album.MoneyIncome = Random.Range(1, 100);
        }
    }
}