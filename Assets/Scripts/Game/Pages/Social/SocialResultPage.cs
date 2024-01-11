using Enums;
using Game.Analyzers;
using MessageBroker.Messages.Production;
using UnityEngine;
using Models.Info;

namespace Game.Pages.Social
{
    /// <summary>
    /// Страница результата социального действия
    /// </summary>
    public abstract class SocialResultPage : Page
    {
        [Header("Анализатор")] 
        [SerializeField] protected SocialAnalyzer analyzer;

        private SocialInfo _social;
        
        public void ShowPage(SocialInfo social)
        {
            _social = social;

            if (social.Type != SocialType.Trends)
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
        private void SaveResult(SocialInfo social)
        {
            GameManager.Instance.MessageBroker.Publish(new ProductionRewardEvent
            {
                MoneyIncome = -social.CharityAmount,
                HypeIncome = social.HypeIncome,
                Exp = settings.SocialsRewardExp,
                WithSocialCooldown = true
            });
        }

        protected override void AfterPageClose()
        {
            SaveResult(_social);
            _social = null;
        }
    }
}