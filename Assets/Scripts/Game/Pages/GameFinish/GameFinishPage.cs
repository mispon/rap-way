using Core;

namespace Game.Pages.GameFinish
{
    public class GameFinishPage : Page
    {
        protected override void AfterPageOpen()
        {
            SoundManager.Instance.PlayGameEnd();
        }
    }
}