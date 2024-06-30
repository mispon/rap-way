using MessageBroker;
using MessageBroker.Interfaces;
using MessageBroker.Messages.Player;
using MessageBroker.Messages.SocialNetworks;
using MessageBroker.Messages.Time;
using MessageBroker.Messages.UI;
using UI.Enums;
using UniRx;
using PlayerAPI = Game.Player.PlayerPackage;

namespace Game.Player.Team
{
    public class TeamMessagesHandler : IMessagesHandler
    {
        public void RegisterHandlers(CompositeDisposable disposable)
        {
            HandleDayLeft(disposable);
            HandleMonthLeft(disposable);
        }

        private static void HandleDayLeft(CompositeDisposable disposable)
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

        private static void HandleMonthLeft(CompositeDisposable disposable)
        {
            MsgBroker.Instance
                .Receive<MonthLeftMessage>()
                .Subscribe(e => OnMonthLeft(e.Month))
                .AddTo(disposable);
        }

        private static void DecreaseManagersCooldown()
        {
            var manager = PlayerAPI.Data.Team.Manager;
            if (manager.Cooldown > 0)
            {
                manager.Cooldown -= 1;
            }

            var prManager = PlayerAPI.Data.Team.PrMan;
            if (prManager.Cooldown > 0)
            {
                prManager.Cooldown -= 1;
            }
        }

        private static void OnMonthLeft(int month)
        {
            if (month % 3 != 0)
            {
                return;
            }

            var teammates = TeamManager.Instance.GetTeammates(e => !e.IsEmpty);
            if (teammates.Length == 0)
            {
                return;
            }

            foreach (var teammate in teammates)
            {
                teammate.HasPayment = false;
            }

            // TODO: remove this message and send single month finance report 
            const int teamTab = 3;
            MsgBroker.Instance.Publish(new EmailMessage
            {
                Title       = "email_team_salary_title",
                Content     = "email_team_salary_content",
                ContentArgs = new[] {PlayerAPI.Data.Info.NickName},
                Sender      = "team.assistant@mail.com",
                mainAction = () =>
                {
                    MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.Training, teamTab));
                }
            });

            MsgBroker.Instance.Publish(new TeamSalaryMessage());
        }
    }
}