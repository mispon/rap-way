using Core;
using ScriptableObjects;
using UI.Windows.GameScreen;

namespace UI.Windows.Pages.GameFinish
{
    public class GameFinishPage : Page
    {
        protected override void AfterPageOpen()
        {
            SoundManager.Instance.PlaySound(UIActionType.GameEnd);
        }
    }
}