using MessageBroker;
using MessageBroker.Messages.UI;
using UI.Enums;
using UniRx;

namespace UI.Controls.UnreadBadge
{
    public class SocialsUnreadBadge : BaseUnreadBadge
    {
        protected override void RegisterHandlers()
        {
            MsgBroker.Instance
                .Receive<WindowControlMessage>()
                .Subscribe(e => HandlePageOpen(e.Type))
                .AddTo(disposables);

            // todo: 
        }

        private void HandlePageOpen(WindowType type)
        {
            if (type != WindowType.SocialNetworks)
                return;

            HideBadge();
        }
    }
}