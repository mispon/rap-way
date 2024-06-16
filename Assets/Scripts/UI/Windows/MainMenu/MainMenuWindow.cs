// using Enums;
// using Firebase.Analytics;
using Game;
using MessageBroker;
using MessageBroker.Messages.UI;
using Sirenix.OdinInspector;
using UI.Base;
using UI.Controls;
using UI.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.MainMenu
{
    public class MainMenuWindow : CanvasUIElement
    {
        [BoxGroup("Buttons"), SerializeField]    private Button _continueGameButton;
        [BoxGroup("Animations"), SerializeField] private ProductionAnim _productionAnim;
 
        protected override void BeforeShow(object ctx = null)
        {
            if (!GameManager.Instance.HasAnySaves())
            {
                // FirebaseAnalytics.LogEvent(FirebaseGameEvents.GameFirstOpen);
                MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.LangSelection));
            } else if (GameManager.Instance.NeedAskReview())
            {
                MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.AskReview));
            }

            _continueGameButton.interactable = GameManager.Instance.HasCharacter();
        }

        public override void Show(object ctx = null)
        {
            base.Show(ctx);
            _productionAnim.Refresh();
        }
    }
}