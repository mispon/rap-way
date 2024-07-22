using Core;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

            emailsButton.onClick.AddListener(() => MsgBroker.Instance.Publish(new EmailMessage
            {
                Title = "concert_event_name_10",
                Content = "concert_event_desc_10",
                Sender = "plaki-plaki@mail.kz",
                Sprite = SpritesManager.Instance.GetRandom()
            }));
        }
    }
}