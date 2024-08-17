using Core;
using Core.Context;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Achievement
{
    public class NewAchievementsPage : Page
    {
        [SerializeField] private Text achievement;

        public override void Show(object ctx = null)
        {
            var value = ctx.Value<string>();
            achievement.text = value;

            base.Show(ctx);
        }

        protected override void AfterShow(object ctx = null)
        {
            SoundManager.Instance.PlaySound(UIActionType.Achieve);
        }

        protected override void AfterHide()
        {
            achievement.text = "";
        }
    }
}