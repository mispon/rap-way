using MessageBroker;
using MessageBroker.Messages.Time;
using UniRx;

namespace Game.Labels
{
    /// <summary>
    /// Music labels events handler
    /// </summary>
    public partial class LabelsPackage
    {
        protected override void RegisterHandlers()
        {
            HandleWeekLeft();
            HandleMonthLeft();
        }

        private void HandleWeekLeft()
        {
            MainMessageBroker.Instance
                .Receive<WeekLeftEvent>()
                .Subscribe(e =>
                {
                    UpdateLabelsStats();
                })
                .AddTo(disposable);
        }
        
        private void HandleMonthLeft()
        {
            MainMessageBroker.Instance
                .Receive<MonthLeftEvent>()
                .Subscribe(e =>
                {
                    if (e.Month % _settings.Labels.RappersActionsFrequency == 0)
                        RandomRapperLabelAction();
            
                    if (e.Month % _settings.Labels.InvitePlayerFrequency == 0)
                        InvitePlayerToLabel();
                    
                    SendPlayersLabelIncomeNotification();
                })
                .AddTo(disposable);
        }
    }
}