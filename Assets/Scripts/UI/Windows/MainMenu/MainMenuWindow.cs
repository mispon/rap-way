// using Firebase.Analytics;
using Game;
using MessageBroker;
using MessageBroker.Messages.UI;
using UI.Base;
using UI.Controls;
using UI.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.MainMenu
{
    public class MainMenuWindow : CanvasUIElement
    {
        [SerializeField] private Button _continueGameButton;
        [SerializeField] private ProductionAnim _productionAnim;
 
        protected override void BeforeShow(object ctx = null)
        {
            _continueGameButton.interactable = GameManager.Instance.HasCharacter();
            
            if (!GameManager.Instance.HasAnySaves())
            {
                // FirebaseAnalytics.LogEvent(FirebaseGameEvents.GameFirstOpen);
                MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.LangSelection));
                return;
            } 
            
            if (GameManager.Instance.NeedAskReview())
            {
                MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.AskReview));
            }
        }

        public override void Show(object ctx = null)
        {
            base.Show(ctx);
            _productionAnim.Refresh();
        }
    }
}