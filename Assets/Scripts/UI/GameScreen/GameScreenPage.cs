using MessageBroker;
using MessageBroker.Messages.Player.State;
using UI.Windows.GameScreen;

namespace UI.GameScreen
{
    public class GameScreenPage : Page
    {
        protected override void BeforeShow(object ctx = null)
        {
            MsgBroker.Instance.Publish(new FullStateRequest());
        }
    }
}