using Core;
using Enums;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using MessageBroker.Messages.UI;
using UI.Enums;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using PlayerAPI = Game.Player.PlayerPackage;
using LabelsAPI = Game.Labels.LabelsPackage;

namespace UI.GameScreen
{
    public class GameScreenTestPad : MonoBehaviour
    {
        [SerializeField] private Button newsButton;
        [SerializeField] private Button emailsButton;

        private void Start()
        {
            newsButton.onClick.AddListener(() => MsgBroker.Instance.Publish(new NewsMessage
            {
                Text = "eagle_negative_5",
                Popularity = Random.Range(100, 1000),
                Sprite = SpritesManager.Instance.GetRandom()
            }));

            emailsButton.onClick.AddListener(() =>
            {
                MsgBroker.Instance.Publish(new EmailMessage
                {
                    Type = EmailsType.TeamSalary,
                    Title = "email_team_salary_title",
                    Content = "email_team_salary_content",
                    ContentArgs = new[] { PlayerAPI.Data.Info.NickName },
                    Sender = "team.assistant@mail.com",
                    mainAction = () =>
                    {
                        MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.Training, 3));
                    }
                });
            });
        }
    }
}