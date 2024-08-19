using Core.Context;
using Game.Rappers.Desc;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using MessageBroker.Messages.Time;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;
using RappersAPI = Game.Rappers.RappersPackage;
using LabelsAPI = Game.Labels.LabelsPackage;

namespace UI.Windows.GameScreen.Team
{
    public class RapperJoinLabelPage : Page
    {
        [SerializeField] private Text rapper;
        [SerializeField] private Image avatar;

        private RapperInfo _rapper;

        public override void Show(object ctx = null)
        {
            _rapper = ctx.Value<RapperInfo>();

            rapper.text = _rapper.Name.ToUpper();
            avatar.sprite = _rapper.Avatar;

            base.Show(ctx);
        }

        protected override void AfterShow(object ctx = null)
        {
            MsgBroker.Instance.Publish(new TimeFreezeMessage { IsFreezed = true });
            EnrollRapperToLabel();
        }

        protected override void BeforeHide()
        {
            MsgBroker.Instance.Publish(new TimeFreezeMessage { IsFreezed = false });
        }

        private void EnrollRapperToLabel()
        {
            // TODO: create JoinLabel func in LabelsAPI with sending news event?

            _rapper.Label = PlayerAPI.Data.Label;
            LabelsAPI.Instance.RefreshScore(_rapper.Label);

            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text = "news_rapper_join_label",
                TextArgs = new[]
                {
                    _rapper.Name,
                    PlayerAPI.Data.Label
                },
                Sprite = _rapper.Avatar,
                Popularity = RappersAPI.GetFansCount(_rapper)
            });
        }
    }
}