using MessageBroker;
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
            MainMessageBroker.Instance
                .Receive<DayLeftMessage>()
                .Subscribe()
                .AddTo(disposable);
        }
    }
}