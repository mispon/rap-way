using Core.Context;
using Enums;
using Game.Production.Analyzers;
using MessageBroker;
using MessageBroker.Messages.Production;
using Models.Production;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Windows.GameScreen.Social
{
    public abstract class SocialResultPage : Page
    {
        [BoxGroup("Analyzer"), SerializeField] protected SocialAnalyzer analyzer;

        private SocialInfo _social;
        
        public override void Show(object ctx = null)
        {
            _social = ctx.Value<SocialInfo>();

            if (_social.Type != SocialType.Trends)
                analyzer.Analyze(_social);
            
            DisplayResult(_social);
            base.Show(ctx);
        }

        protected abstract void DisplayResult(SocialInfo socialInfo);

        private void SaveResult(SocialInfo social)
        {
            MsgBroker.Instance.Publish(new ProductionRewardMessage
            {
                MoneyIncome = -social.CharityAmount,
                HypeIncome = social.HypeIncome,
                Exp = settings.Socials.RewardExp,
                WithSocialCooldown = true
            });
        }

        protected override void AfterHide()
        {
            SaveResult(_social);
            _social = null;
        }
    }
}