using Core;
using Data;

namespace Game.Pages.GameFinish
{
    public class GameFinishPage : Page
    {
        protected override void AfterPageOpen()
        {
            SoundManager.Instance.PlaySound(UIActionType.GameEnd);
        }
    }
}