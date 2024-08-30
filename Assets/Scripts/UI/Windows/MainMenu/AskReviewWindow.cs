using System.Runtime.InteropServices;
using Game;
using MessageBroker;
using MessageBroker.Messages.UI;
using UI.Base;
using UI.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.MainMenu
{
    public class AskReviewWindow : CanvasUIElement
    {
        [SerializeField] private string reviewPageURL;
        [SerializeField] private Button reviewButton;

        [DllImport("__Internal")]
        private static extern void YandexRateGame();

        private void Start()
        {
            reviewButton.onClick.AddListener(OpenReviewPage);
        }

        public override void Show(object ctx = null)
        {
            GameManager.Instance.GameStats.AskedReview = true;
            base.Show(ctx);
        }

        private void OpenReviewPage()
        {
#if UNITY_ANDROID
            Application.OpenURL(reviewPageURL);
#elif UNITY_WEBGL
            YandexRateGame();
#endif  

            MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.MainMenu));
        }
    }
}