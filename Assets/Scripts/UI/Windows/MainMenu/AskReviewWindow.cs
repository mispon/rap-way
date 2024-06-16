using Game;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.MainMenu
{
    public class AskReviewWindow : CanvasUIElement
    {
        [SerializeField] private string reviewPageURL;
        [SerializeField] private Button reviewButton;

        protected override void AfterShow(object ctx = null)
        {
            reviewButton.onClick.AddListener(() => Application.OpenURL(reviewPageURL));
        }

        public override void Show(object ctx = null)
        {
            GameManager.Instance.GameStats.AskedReview = true;
        }
    }
}