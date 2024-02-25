using MessageBroker;
using MessageBroker.Handlers;
using MessageBroker.Messages.Time;
using UniRx;

namespace Game.Player.Energy
{
    public class EnergyMessagesHandler : BaseMessagesHandler
    {
        protected override void RegisterHandlers()
        {
            HandleDayLeft();
        }

        private void HandleDayLeft()
        {
            MsgBroker.Instance
                .Receive<DayLeftMessage>()
                .Subscribe()
                .AddTo(disposable);
        }
    }
}