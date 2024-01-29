using System.Collections;
using Enums;
using Firebase.Analytics;
using Game;
using Sirenix.OdinInspector;
using UI.Base;
using UI.Controls;
using UI.Enums;
using UI.MessageBroker;
using UI.MessageBroker.Messages;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.MainMenu
{
    public class MainMenuWindow : CanvasUIElement
    {
        [BoxGroup("Buttons")] [SerializeField] private Button _continueGameButton;
        [BoxGroup("Animations")] [SerializeField] private ProductionAnim _productionAnim;

        public override void Initialize()
        {
            base.Initialize();

            if (GameManager.Instance.HasAnySaves())
            {
                FirebaseAnalytics.LogEvent(FirebaseGameEvents.GameFirstOpen);
            }

            if (GameManager.Instance.NeedAskReview())
            {
                UIMessageBroker.Instance.Publish(new WindowControlMessage {Type = WindowType.AskReview});
            }

            StartCoroutine(SetupContinueButton());
        }

        public override void Show()
        {
            base.Show();
            _productionAnim.Refresh();
        }

        private IEnumerator SetupContinueButton()
        {
            yield return new WaitUntil(() => GameManager.Instance.IsReady);
            _continueGameButton.interactable = GameManager.Instance.HasCharacter();
        }
    }
}