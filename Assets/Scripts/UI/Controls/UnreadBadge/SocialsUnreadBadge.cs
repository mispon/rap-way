using System.Linq;
using Game;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using UniRx;

namespace UI.Controls.UnreadBadge
{
    public class SocialsUnreadBadge : BaseUnreadBadge
    {
        protected override void RegisterHandlers()
        {
            MsgBroker.Instance
                .Receive<EmailMessage>()
                .Subscribe(e => IncCounter())
                .AddTo(disposables);
            MsgBroker.Instance
                .Receive<ReadEmailMessage>()
                .Subscribe(e => DecCounter())
                .AddTo(disposables);
        }

        protected override void Init()
        {
            var total = 0;

            total += GameManager.Instance.Emails.Count(e => e.IsNew);
            // TODO: add twitter mentions from other rappers

            UpdateCounter(total);
        }
    }
}