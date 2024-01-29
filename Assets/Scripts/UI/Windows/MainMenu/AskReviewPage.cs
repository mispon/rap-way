using Game;
using UI.Windows.Pages;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.MainMenu
{
    public class AskReviewPage : Page
    {
        [SerializeField] private string reviewPageURL;
        [SerializeField] private Button reviewButton;
        [SerializeField] private Button skipButton;

        private void Start()
        {
            reviewButton.onClick.AddListener(MakeReview);
            skipButton.onClick.AddListener(SkipReview);
        }

        protected override void AfterPageOpen()
        {
            GameManager.Instance.GameStats.AskedReview = true;
        }

        private void MakeReview()
        {
            Application.OpenURL(reviewPageURL);
            Close();
        }
        
        private void SkipReview()
        {
            // :(
            Close();
        }
    }
}