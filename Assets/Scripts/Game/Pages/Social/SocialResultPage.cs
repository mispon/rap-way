using Game.Analyzers;
using UnityEngine;
using Models.Info;

namespace Game.Pages.Social
{
    /// <summary>
    /// Страница результата социального действия
    /// </summary>
    public abstract class SocialResultPage : Page
    {
        private const int SOCIAL_COOLDOWN = 5;

        [Header("Анализатор")] 
        [SerializeField] protected SocialAnalyzer analyzer;

        private SocialInfo _social;
        
        public void ShowPage(SocialInfo social)
        {
            _social = social;
            analyzer.Analyze(_social);
            
            Open();
            DisplayResult(social);
        }

        /// <summary>
        /// Отображает результаты соц. действия
        /// </summary>
        protected abstract void DisplayResult(SocialInfo socialInfo);
        
        /// <summary>
        /// Сохраняет результат социального действия
        /// </summary>
        private static void SaveResult(SocialInfo social)
        {
            GameManager.Instance.GameStats.SocialsCooldown = SOCIAL_COOLDOWN;
            PlayerManager.Instance.SpendMoney(social.CharityAmount);
            PlayerManager.Instance.AddHype(social.HypeIncome);
        }

        protected override void AfterPageClose()
        {
            SaveResult(_social);
            _social = null;
        }
    }
}

