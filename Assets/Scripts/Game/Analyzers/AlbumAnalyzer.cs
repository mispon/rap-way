using Models.Info.Production;
using UnityEngine;

namespace Game.Analyzers
{
    /// <summary>
    /// Анализатор альбома
    /// </summary>
    public class AlbumAnalyzer : Analyzer<AlbumInfo>
    {
        [Header("Кривая ценности одного очка работы от количества фанатов")] 
        [SerializeField] private AnimationCurve fansToPointsIncomeCurve;
        
        [Header("Настройки тренда")] 
        [SerializeField, Tooltip("Анализатор совпадения с трендом")] 
        private TrendAnalyzer trendAnalyzer;
        [SerializeField, Tooltip("Коэффициент совпадения с трендом")] 
        private float trendsEquality;

        [Header("Настройки слушателей")] 
        [SerializeField, Tooltip("Зависимость числа слушателей от очков")] 
        private AnimationCurve listenAmountCurve;
        [SerializeField, Tooltip("Зависимость места в чарте от количества слушателей")] 
        private AnimationCurve chartPositionCurve;

        [Header("Настройки прироста фанатов")] 
        [SerializeField, Tooltip("Зависимость коэффициента прироста фанатов от текущих фанатов")] 
        private AnimationCurve fansIncomeCurve;
        [SerializeField, Tooltip("Коэффициент влияния хайпа на прирост фанатов")] private float hypeImpactMultiplier;

        [Header("Коэффициент заработка от количества прослушиваний")] 
        [SerializeField] private int moneyIncomeMultiplier;
        
        /// <summary>
        /// Анализирует успешность альбома 
        /// </summary>
        public override void Analyze(AlbumInfo album)
        {
            var totalFans = PlayerManager.Data.Fans;
            
            var resultPoints = fansToPointsIncomeCurve.Evaluate(totalFans) * (album.TextPoints + album.BitPoints);
            
            trendAnalyzer.Analyze(album.TrendInfo);//записывает в album.TrendInfo.EqualityValue коэффициент совпадения от 0 до 1
            resultPoints += resultPoints * Mathf.Lerp(0, trendsEquality, album.TrendInfo.EqualityValue);

            album.ListenAmount = (int) listenAmountCurve.Evaluate(resultPoints);
            album.ChartPosition = (int) chartPositionCurve.Evaluate(album.ListenAmount);

            var hypeImpact = hypeImpactMultiplier * PlayerManager.Data.Hype;
            var fansIncomeFromListeners = fansIncomeCurve.Evaluate(totalFans) * album.ListenAmount; 
            album.FansIncome = (int) (hypeImpact * fansIncomeFromListeners);
            album.MoneyIncome = moneyIncomeMultiplier * album.ListenAmount;
        }
    }
}