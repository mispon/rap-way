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
    ///     Player's label logic
    /// </summary>
    public partial class LabelsPackage
    {
        /// <summary>
        ///     Link on player's label
        /// </summary>
        public LabelInfo PlayerLabel
        {
            get => GameManager.Instance.PlayerLabel;
            private set => GameManager.Instance.PlayerLabel = value;
        }

        /// <summary>
        ///     Has player his own label or not
        /// </summary>
        public bool IsPlayerLabelEmpty => PlayerLabel == null || string.IsNullOrWhiteSpace(PlayerLabel.Name);

        /// <summary>
        ///     Creates player's label
        /// </summary>
        public void CreatePlayersLabel(LabelInfo label)
        {
            PlayerLabel                      = label;
            GameManager.Instance.PlayerLabel = label;
        }

        /// <summary>
        ///     Removes player's label
        /// </summary>
        public void DisbandPlayersLabel()
        {
            PlayerLabel                      = null;
            GameManager.Instance.PlayerLabel = null;
        }

        /// <summary>
        ///     Returns label income from its members
        /// </summary>
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
            return members.Sum(e => RappersAPI.GetFansCount(e) / 100 * incomePercent);
        }

        /// <summary>
        ///     Sends labels money report notification
        /// </summary>
        private void SendPlayersLabelIncomeEmail()
        {
            if (IsPlayerLabelEmpty)
            {
                return;
            }

            // TODO: remove this message and send single month finance report 
            MsgBroker.Instance.Publish(new EmailMessage
            {
                Title       = "email_label_money_report_title",
                Content     = "email_label_money_report_content",
                ContentArgs = new[] {PlayerAPI.Data.Info.NickName},
                Sender      = "fin.assistant@mail.com",
                mainAction = () =>
                {
                    MsgBroker.Instance.Publish(new WindowControlMessage
                    {
                        Type    = WindowType.Personal,
                        Context = PersonalTabType.MoneyReport
                    });
                }
            });
        }
    }
}