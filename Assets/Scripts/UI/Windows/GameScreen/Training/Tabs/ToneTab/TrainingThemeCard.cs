using System;
using Enums;
using ScriptableObjects;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Training.Tabs.ToneTab
{
    public class TrainingThemeCard : TrainingToneCard
    {
        private Themes value => ((ThemesInfo) _info).Type;

        protected override Enum GetValue()
        {
            return value;
        }

        protected override bool IsLocked()
        {
            return !PlayerAPI.Data.Themes.Contains(value);
        }
    }
}