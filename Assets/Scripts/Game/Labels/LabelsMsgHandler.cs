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
            MsgBroker.Instance
                .Receive<WeekLeftMessage>()
                .Subscribe(e =>
                {
                    UpdateLabelsStats();
                })
                .AddTo(disposable);
        }
        
        private void HandleMonthLeft()
        {
            MsgBroker.Instance
                .Receive<MonthLeftMessage>()
                .Subscribe(e =>
                {
                    if (e.Month % _settings.Labels.RappersActionsFrequency == 0)
                        RandomRapperLabelAction();
            
                    if (e.Month % _settings.Labels.InvitePlayerFrequency == 0)
                        InvitePlayerToLabel();
                    
                    SendPlayersLabelIncomeEmail();
                })
                .AddTo(disposable);
        }
    }
}