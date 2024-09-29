using System;
using System.Linq;
using Game.Labels.Desc;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using MessageBroker.Messages.UI;
using UI.Enums;
using UI.Windows.GameScreen.Personal;
using RappersAPI = Game.Rappers.RappersPackage;
using PlayerAPI = Game.Player.PlayerPackage;

namespace Game.Labels
{
    /// <summary>
    /// Player's label logic
    /// </summary>
    public partial class LabelsPackage
    {
        public LabelInfo PlayerLabel
        {
            get => GameManager.Instance.PlayerLabel;
            private set => GameManager.Instance.PlayerLabel = value;
        }

        public bool IsPlayerLabelEmpty => PlayerLabel == null || string.IsNullOrWhiteSpace(PlayerLabel.Name);

        public void CreatePlayersLabel(LabelInfo label)
        {
            PlayerLabel = label;
            GameManager.Instance.PlayerLabel = label;
        }

        public void DisbandPlayersLabel()
        {
            PlayerLabel = null;
            GameManager.Instance.PlayerLabel = null;
        }

        public int GetPlayersLabelIncome()
        {
            if (IsPlayerLabelEmpty)
            {
                return 0;
            }

            var members = RappersAPI.Instance
                .GetAll()
                .Where(e => string.Equals(e.Label, PlayerLabel.Name, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            const int incomePercent = 5;
            return members.Sum(e => e.Fans / 100 * incomePercent);
        }

        private void SendPlayersLabelIncomeEmail()
        {
            if (IsPlayerLabelEmpty)
            {
                return;
            }

            // TODO: remove this message and send single month finance report 
            MsgBroker.Instance.Publish(new EmailMessage
            {
                Title = "email_label_money_report_title",
                Content = "email_label_money_report_content",
                ContentArgs = new[] { PlayerAPI.Data.Info.NickName },
                Sender = "fin.assistant@mail.com",
                mainAction = () =>
                {
                    MsgBroker.Instance.Publish(new WindowControlMessage
                    {
                        Type = WindowType.Personal,
                        Context = PersonalTabType.MoneyReport
                    });
                }
            });
        }
    }
}