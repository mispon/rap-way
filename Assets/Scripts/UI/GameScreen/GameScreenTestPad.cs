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
using MessageBroker.Messages.Player.State;

namespace UI.GameScreen
{
    public class GameScreenTestPad : MonoBehaviour
    {
        [SerializeField] private Button newsButton;
        [SerializeField] private Button emailsButton;
        [SerializeField] private Button moneyButton;

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
                var label = LabelsAPI.Instance.GetRandom();

                MsgBroker.Instance.Publish(new EmailMessage
                {
                    Type = EmailsType.LabelsContract,
                    Title = "label_contract_greeting",
                    TitleArgs = new[] { label.Name },
                    Content = "label_contract_text",
                    ContentArgs = new[]
                    {
                        PlayerAPI.Data.Info.NickName,
                        label.Name,
                        label.Name
                    },
                    Sender = $"{label.Name.ToLower()}@label.com",
                    Sprite = label.Logo,
                    mainAction = () =>
                    {
                        MsgBroker.Instance.Publish(new WindowControlMessage
                        {
                            Type = WindowType.LabelContract,
                            Context = label
                        });
                    }
                });
            });

            moneyButton.onClick.AddListener(() =>
            {
                MsgBroker.Instance.Publish(new ChangeMoneyMessage { Amount = 100 });
            });
        }
    }
}