using System;
using System.Linq;
using Models.Production;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.History.HistoryProduction
{
    [Serializable]
    public class HistoryAlbumController: HistoryProductionController
    {
        protected override ProductionBase[] PlayerProductionInfos()
            => PlayerAPI.Data.History.AlbumList
                .OrderByDescending(album => album.Id)
                .ToArray();
    }
}