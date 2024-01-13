using System.Collections;
using Core;
using Firebase.Analytics;
using Game.UI.Enums;
using Game.UI.Messages;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Windows
{
    public class MainMenuWindow : CanvasUIElement
    {
        [BoxGroup("Buttons")] [SerializeField] private Button _continueGameButton;
        [BoxGroup("Animations")] [SerializeField] private ProductionAnim _productionAnim;

        public override void Initialize()
        {
            base.Initialize();
            
            if (GameManager.Instance.HasAnySaves())
                FirebaseAnalytics.LogEvent(FirebaseGameEvents.GameFirstOpen);
            
            if (!GameManager.Instance.GameStats.AskedReview && GameManager.Instance.PlayerData.Fans > 0)
                uiMessageBus.Publish(new WindowControlMessage
                {
                    Type = WindowType.AskReview
                });

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