using MessageBroker;
using MessageBroker.Messages.Time;
using UniRx;

namespace Game.Rappers
{
    /// <summary>
    ///     Rappers specific events handler
    /// </summary>
    public partial class RappersPackage
    {
        protected override void RegisterHandlers()
        {
            HandleDayLeft();
        }

        private void HandleDayLeft()
        {
            MsgBroker.Instance
                .Receive<DayLeftMessage>()
                .Subscribe(e => { TriggerAIActions(); })
                .AddTo(disposable);
        }
    }
}