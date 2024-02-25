using System;
using Game;
using UI.Base;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.MainMenu
{
    public class AskReviewWindow : CanvasUIElement
    {
        [SerializeField] private string reviewPageURL;
        [SerializeField] private Button reviewButton;

        private IDisposable _disposable;
        
        protected override void AfterShow()
        {
            _disposable = reviewButton
                .OnClickAsObservable()
                .Subscribe(_ => Application.OpenURL(reviewPageURL));
        }

        public override void Show(object ctx)
        {
            GameManager.Instance.GameStats.AskedReview = true;
        }

        protected override void AfterHide()
        {
            _disposable?.Dispose();
        }
    }
}