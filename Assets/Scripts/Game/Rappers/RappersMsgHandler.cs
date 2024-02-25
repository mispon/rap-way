using MessageBroker;
using MessageBroker.Messages.Time;
using UniRx;

namespace Game.Rappers
{
    /// <summary>
    /// Rappers specific events handler
    /// </summary>
    public partial class RappersPackage
    {
        protected override void RegisterHandlers()
        {
            HandleMonthLeft();
        }
        
        private void HandleMonthLeft()
        {
            MainMessageBroker.Instance
                .Receive<MonthLeftMessage>()
                .Subscribe(e =>
                {
                    if (e.Month % _settings.Rappers.FansUpdateFrequency == 0)
                    {
                        RandomlyChangeFans();
                    }
                })
                .AddTo(disposable);
        }
    }
}