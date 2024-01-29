using Game;
using UI.Base;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.MainMenu
{
    public class AskReviewWindow : CanvasUIElement
    {
        [SerializeField] private string _reviewPageURL;
        [SerializeField] private Button _reviewButton;

        protected override void SetupListenersOnInitialize()
        {
            _reviewButton.OnClickAsObservable()
                .Subscribe(_ => Application.OpenURL(_reviewPageURL));
        }

        public override void Show()
        {
            GameManager.Instance.GameStats.AskedReview = true;
        }
    }
}