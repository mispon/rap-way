using MessageBroker;
using MessageBroker.Messages.Time;
using UniRx;

namespace Game.Player.Energy
{
    public class EnergyEventsHandler : BaseEventsHandler
    {
        protected override void RegisterHandlers()
        {
            HandleDayLeft();
        }

        private void HandleDayLeft()
        {
            MainMessageBroker.Instance
                .Receive<DayLeftEvent>()
                .Subscribe()
                .AddTo(disposable);
        }
    }
}