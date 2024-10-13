using MessageBroker;
using MessageBroker.Messages.Labels;
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
            HandleLabelInvite();
        }

        private void HandleDayLeft()
        {
            MsgBroker.Instance
                .Receive<DayLeftMessage>()
                .Subscribe(e => { TriggerAIActions(); })
                .AddTo(disposable);
        }

        private void HandleLabelInvite()
        {
            MsgBroker.Instance
                .Receive<LabelInviteRapperMessage>()
                .Subscribe(e => { _rappersAI.TryJoinLabel(e.RapperId, e.LabelName, _settings); })
                .AddTo(disposable);
        }
    }
}