using MessageBroker;
using MessageBroker.Interfaces;
using MessageBroker.Messages.Time;
using UniRx;

namespace Game.Player.Energy
{
    public class EnergyMessagesHandler : IMessagesHandler
    {
        public void RegisterHandlers(CompositeDisposable disposable)
        {
            HandleDayLeft(disposable);
        }

        private static void HandleDayLeft(CompositeDisposable disposable)
        {
            MsgBroker.Instance
                .Receive<DayLeftMessage>()
                .Subscribe()
                .AddTo(disposable);
        }
    }
}