using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Windows
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