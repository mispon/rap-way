using Core;
using ScriptableObjects;

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