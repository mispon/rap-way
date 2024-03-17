using Enums;
using Game.Player.State;
using Game.Player.State.Desc;
using Models.Trends;

namespace Game.Player
{
    public partial class PlayerPackage
    {
        public static PlayerData Data => GameManager.Instance.PlayerData;

        // TODO: add more different API categories
        public static readonly StateAPI State = new();
        
        public static void UpdateKnownTrends(Styles style, Themes theme)
        {
            Data.LastKnownTrends ??= Trends.New;

            Data.LastKnownTrends.Style = style;
            Data.LastKnownTrends.Theme = theme;
        }
    }
}