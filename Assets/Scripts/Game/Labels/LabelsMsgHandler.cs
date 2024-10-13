using MessageBroker;
using MessageBroker.Messages.Time;
using UniRx;

namespace Game.Labels
{
    /// <summary>
    ///     Music labels events handler
    /// </summary>
    public partial class LabelsPackage
    {
        protected override void RegisterHandlers()
        {
            HandleDayLeft();
            HandleMonthLeft();
        }

        private void HandleDayLeft()
        {
            MsgBroker.Instance
                .Receive<DayLeftMessage>()
                .Subscribe(e =>
                {
                    TriggerAIAction();
                    UpdateLabelsStats();
                })
                .AddTo(disposable);
        }

        private void HandleMonthLeft()
        {
            MsgBroker.Instance
                .Receive<MonthLeftMessage>()
                .Subscribe(e => { SendPlayersLabelIncomeEmail(); })
                .AddTo(disposable);
        }
    }
}