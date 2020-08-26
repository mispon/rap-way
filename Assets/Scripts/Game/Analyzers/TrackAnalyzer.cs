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
        [Header("Кривая ценности одного очка работы от количества фанатов")] 
        [SerializeField] private AnimationCurve fansToPointsIncomeCurve;
        
        [Header("Настройки тренда")] 
        [SerializeField, Tooltip("Коэффициент совпадения с трендом")] 
        private float trendsEqualityMultiplier;

        [Header("Настройки слушателей")] 
        [SerializeField, Tooltip("Зависимость числа слушателей от очков")] 
        private AnimationCurve listenAmountCurve;
        [SerializeField, Tooltip("Зависимость места в чарте от количества слушателей")] 
        private AnimationCurve chartPositionCurve;

        [Header("Настройки прироста фанатов")] 
        [SerializeField, Tooltip("Зависимость коэффициента прироста фанатов от текущих фанатов")] 
        private AnimationCurve fansIncomeCurve;
        [SerializeField, Tooltip("Коэффициент влияния хайпа на прирост фанатов")] 
        private float hypeImpactMultiplier;

        [Header("Коэффициент заработка от количества прослушиваний")] 
        [SerializeField] private int moneyIncomeMultiplier;

        /// <summary>
        /// Анализирует успешность трека
        /// </summary>
        public override void Analyze(TrackInfo track)
        {
            var totalFans = PlayerManager.Data.Fans;
            
            GameStatsManager.Analyze(track.TrendInfo);
            
            var resultPoints = fansToPointsIncomeCurve.Evaluate(totalFans) * (track.TextPoints + track.BitPoints);
            resultPoints += resultPoints * Mathf.Lerp(0, trendsEqualityMultiplier, track.TrendInfo.EqualityValue);

            track.ListenAmount = (int) listenAmountCurve.Evaluate(resultPoints);
            track.ChartPosition = (int) chartPositionCurve.Evaluate(track.ListenAmount);

            var hypeImpact = hypeImpactMultiplier * PlayerManager.Data.Hype;
            var fansIncomeFromListeners = fansIncomeCurve.Evaluate(totalFans) * track.ListenAmount; 
            track.FansIncome = (int) (hypeImpact * fansIncomeFromListeners);
            track.MoneyIncome = moneyIncomeMultiplier * track.ListenAmount;
        }
    }
}