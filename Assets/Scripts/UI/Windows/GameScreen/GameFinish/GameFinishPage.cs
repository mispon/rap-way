using Core;
using ScriptableObjects;

namespace UI.Windows.GameScreen.GameFinish
{
    public class GameFinishPage : Page
    {
        protected override void AfterShow(object ctx = null)
        {
            SoundManager.Instance.PlaySound(UIActionType.GameEnd);
        }
    }
}