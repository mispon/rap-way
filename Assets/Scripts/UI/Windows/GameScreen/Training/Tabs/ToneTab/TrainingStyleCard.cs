using System;
using Enums;
using ScriptableObjects;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Training.Tabs.ToneTab
{
    public class TrainingStyleCard : TrainingToneCard
    {
        private Styles value => ((StylesInfo) _info).Type;

        protected override Enum GetValue()
        {
            return value;
        }

        protected override bool IsLocked()
        {
            return !PlayerAPI.Data.Styles.Contains(value);
        }
    }
}