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
            HandleWeekLeft();
            HandleMonthLeft();
        }

        private void HandleDayLeft()
        {
            MsgBroker.Instance
                .Receive<DayLeftMessage>()
                .Subscribe(e => { UpdateLabelsStats(); })
                .AddTo(disposable);
        }

        private void HandleWeekLeft()
        {
            MsgBroker.Instance
                .Receive<WeekLeftMessage>()
                .Subscribe(e => { TriggerAIAction(); })
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