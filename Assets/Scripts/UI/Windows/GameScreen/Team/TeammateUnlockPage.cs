using Core.Context;
using Core.Localization;
using Extensions;
using Game.Player.Team.Desc;
using MessageBroker;
using MessageBroker.Messages.Time;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Team
{
    public class TeammateUnlockPage : Page
    {
        [SerializeField] private Text teammateName;
        [SerializeField] private Image avatar;

        public override void Show(object ctx = null)
        {
            var teammate = ctx.ValueByKey<Teammate>("teammate");
            var sprite = ctx.ValueByKey<Sprite>("sprite");

            var nameKey = teammate.Type.GetDescription();
            teammateName.text = LocalizationManager.Instance.Get(nameKey).ToUpper();
            avatar.sprite = sprite;

            base.Show(ctx);
        }

        protected override void AfterShow(object ctx = null)
        {
            MsgBroker.Instance.Publish(new TimeFreezeMessage { IsFreezed = true });
        }

        protected override void BeforeHide()
        {
            MsgBroker.Instance.Publish(new TimeFreezeMessage { IsFreezed = false });
        }
    }
}