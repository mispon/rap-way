using System.Linq;
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
        [Header("Кривая ценности одного очка работы от количества фанатов")] 
        [SerializeField] private AnimationCurve fansToPointsIncomeCurve;

        [Header("Настройки просмоторов")] 
        [SerializeField, Tooltip("Зависимость числа просмотров от очков")] 
        private AnimationCurve viewsCurve;
        [SerializeField, Tooltip("Зависимость коэффициента кол-ва просмотров от успеха трека (номер в чарте)")]
        private AnimationCurve viewsFromTrackCurve;
        [SerializeField, Tooltip("Доля лайков в зависимости от импакта трека")] 
        private AnimationCurve likesFromTrackCurve;
        [SerializeField, Range(0f, 1f), Tooltip("Минимальная доля оценок на количество просмотров")] 
        private float minMarksRatio = 0.5f;
        [SerializeField, Range(0f, 1f), Tooltip("Максимальная доля оценок на количество просмотров")] 
        private float maxMarksRatio = 0.7f;

        [Header("Настройки прироста фанатов")] 
        [SerializeField, Tooltip("Зависимость коэффициента прироста фанатов от текущих фанатов")] 
        private AnimationCurve fansIncomeCurve;
        [SerializeField, Tooltip("Коэффициент влияния хайпа на прирост фанатов")] 
        private float hypeImpactMultiplier;

        [Header("Коэффициент заработка от количества просмотров")] 
        [SerializeField] private int moneyIncomeMultiplier;
        
        /// <summary>
        /// Анализирует успешность клипа
        /// </summary>
        public override void Analyze(ClipInfo clip)
        {
            var totalFans = PlayerManager.Data.Fans;
            var trackInfo = ProductionManager.GetTrack(clip.TrackId);
            
            var trackImpact = viewsFromTrackCurve.Evaluate(trackInfo.ChartPosition);
            var resultPoints = fansToPointsIncomeCurve.Evaluate(totalFans) * (clip.PlayerPoints + clip.OperatorPoints + clip.DirectorPoints);
            
            clip.Views = (int) viewsCurve.Evaluate(resultPoints * trackImpact);
            var marks = clip.Views * Random.Range(minMarksRatio, maxMarksRatio);
            clip.Likes = (int) (marks * likesFromTrackCurve.Evaluate(trackImpact));
            clip.Dislikes = (int) (marks - clip.Likes);

            var hypeImpact = hypeImpactMultiplier * PlayerManager.Data.Hype;
            var fansIncomeFromViews = fansIncomeCurve.Evaluate(totalFans) * clip.Views; 
            clip.FansIncome = (int) (hypeImpact * fansIncomeFromViews);
            clip.MoneyIncome = moneyIncomeMultiplier * clip.Views;
        }
    }
}