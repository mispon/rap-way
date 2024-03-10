using Enums;
using Game.Player.State.Desc;
using Models.Trends;

namespace Game.Player
{
    public partial class PlayerPackage
    {
        public static PlayerData Data => GameManager.Instance.PlayerData;

        /// <summary>
        /// Обновляет информацию о трендах
        /// TODO: move to more appropriate place
        /// </summary>
        public static void UpdateTrends(Styles style, Themes theme)
        {
            Data.LastKnownTrends ??= Trends.New;

            Data.LastKnownTrends.Style = style;
            Data.LastKnownTrends.Theme = theme;
        }
    }
}