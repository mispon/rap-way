using UnityEngine;

namespace UI.Windows.GameScreen
{
    public class GameScreenPage : Page
    {
        [SerializeField] private GameObject particles;

        public override void Show(object ctx = null)
        {
            particles.SetActive(true);
        }

        public override void Hide()
        {
            particles.SetActive(false);
        }
    }
}