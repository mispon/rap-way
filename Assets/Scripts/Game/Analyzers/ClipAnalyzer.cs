using Models.Info.Production;
using UnityEngine;

namespace Game.Analyzers
{
    /// <summary>
    /// Анализатор клипа
    /// </summary>
    public class ClipAnalyzer : Analyzer<ClipInfo>
    {
        [Header("Кривая ценности одного очка работы от количества фанатов")] 
        [SerializeField] private AnimationCurve fansToPointsIncomeCurve;

        [Header("Настройки просмоторов")] 
        [SerializeField, Tooltip("Зависимость числа просмотров от очков")] 
        private AnimationCurve viewsCurve;
        [SerializeField, Range(0f, 1f), Tooltip("Минимальная доля лайков на количество просмотров")] 
        private float minLikesRatio = 0.2f;
        [SerializeField, Range(0f, 1f), Tooltip("Максимальная доля лайков на количество просмотров")] 
        private float maxLikesRatio = 0.2f;

        [Header("Настройки прироста фанатов")] 
        [SerializeField, Tooltip("Зависимость коэффициента прироста фанатов от текущих фанатов")] 
        private AnimationCurve fansIncomeCurve;
        [SerializeField, Tooltip("Коэффициент влияния хайпа на прирост фанатов")] private float hypeImpactMultiplier;

        [Header("Коэффициент заработка от количества просмотров")] 
        [SerializeField] private int moneyIncomeMultiplier;
        
        /// <summary>
        /// Анализирует успешность клипа
        /// </summary>
        public override void Analyze(ClipInfo clip)
        {
            var totalFans = PlayerManager.Data.Fans;
            
            var resultPoints = fansToPointsIncomeCurve.Evaluate(totalFans) * (clip.PlayerPoints + clip.OperatorPoints + clip.DirectorPoints);
            clip.Views = (int) viewsCurve.Evaluate(resultPoints);
            clip.Likes = (int) (clip.Views * Random.Range(minLikesRatio, maxLikesRatio));
            clip.Dislikes = (int) (clip.Views * Random.Range(1 - maxLikesRatio, 1 - minLikesRatio));

            var hypeImpact = hypeImpactMultiplier * PlayerManager.Data.Hype;
            var fansIncomeFromViews = fansIncomeCurve.Evaluate(totalFans) * clip.Views; 
            clip.FansIncome = (int) (hypeImpact * fansIncomeFromViews);
            clip.MoneyIncome = moneyIncomeMultiplier * clip.Views;
        }
    }
}