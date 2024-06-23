using System.Linq;
using Game;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using UniRx;

namespace UI.Controls.UnreadBadge
{
    public class EmailsUnreadBadge : BaseUnreadBadge
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
            var emails = GameManager.Instance.Emails.Count(e => e.IsNew);
            UpdateCounter(emails);
        }
    }
}