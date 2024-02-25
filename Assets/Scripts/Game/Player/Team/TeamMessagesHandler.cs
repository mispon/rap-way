using Core.Events;
using Game.Notifications;
using MessageBroker;
using MessageBroker.Handlers;
using MessageBroker.Messages.Time;
using MessageBroker.Messages.UI;
using UI.Enums;
using UniRx;

namespace Game.Player.Team
{
    public class TeamMessagesHandler : BaseMessagesHandler
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
                    TeamManager.Instance.TryUnlockTeammates();
                    DecreaseManagersCooldown();
                })
                .AddTo(disposable);
        }

        private void HandleMonthLeft()
        {
            MsgBroker.Instance
                .Receive<MonthLeftMessage>()
                .Subscribe(e => OnMonthLeft(e.Month))
                .AddTo(disposable);
        }
        
        private static void DecreaseManagersCooldown()
        {
            var manager = PlayerManager.Data.Team.Manager;
            if (manager.Cooldown > 0)
                manager.Cooldown -= 1;
            
            var prManager = PlayerManager.Data.Team.PrMan;
            if (prManager.Cooldown > 0)
                prManager.Cooldown -= 1;
        }
        
        private static void OnMonthLeft(int month)
        {
            if (month % 3 != 0)
                return;
            
            var teammates = TeamManager.Instance.GetTeammates(e => !e.IsEmpty);
            if (teammates.Length == 0)
                return;

            foreach (var teammate in teammates)
                teammate.HasPayment = false;

            const int teamTab = 3;
            NotificationManager.Instance.AddClickNotification(() =>
            {
                MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.Training, teamTab));
            });
            
            EventManager.RaiseEvent(EventType.UncleSamsParty);
        }
    }
}