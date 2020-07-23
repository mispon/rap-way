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
        [Header("Анализатор совпадения с трендом")] 
        [SerializeField] private TrendAnalyzer trendAnalyzer;
    
        [Header("Коэффициенты очков")] 
        [SerializeField] private float textPointsMultiplier;
        [SerializeField] private float bitmakingPointsMultiplier;
        
        [Header("Коэффициент совпадения с трендом")]
        [SerializeField] private float trendsEquality;

        [Header("Кривая числа слушателей от очков")] 
        [SerializeField]private AnimationCurve listenAmountCurve;
        [Header("Кривая зависимости места в чарте от количества слушателей")]
        [SerializeField] private AnimationCurve chartPositionCurve;
        
        /// <summary>
        /// Анализирует успешность трека
        /// </summary>
        public override void Analyze(TrackInfo track)
        {
            trendAnalyzer.Analyze(track.TrendInfo);

            var resultPoints = textPointsMultiplier * track.TextPoints + bitmakingPointsMultiplier * track.BitPoints;
            resultPoints += resultPoints * Mathf.Lerp(0, trendsEquality, track.TrendInfo.EqualityValue);

            track.ListenAmount = (int) listenAmountCurve.Evaluate(resultPoints);
            track.ChartPosition = (int) chartPositionCurve.Evaluate(track.ListenAmount);
            
            // todo: analyze track
            
//            track.ListenAmount = Random.Range(1, 100);
//            track.ChartPosition = Random.Range(1, 100);
            track.FansIncome = Random.Range(1, 100);
            track.MoneyIncome = Random.Range(1, 100);
        }
    }
}