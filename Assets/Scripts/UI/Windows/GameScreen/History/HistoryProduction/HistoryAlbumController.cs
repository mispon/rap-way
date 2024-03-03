using System;
using System.Linq;
using Game.Player;
using Models.Production;

namespace UI.Windows.GameScreen.History.HistoryProduction
{
    [Serializable]
    public class HistoryAlbumController: HistoryProductionController
    {
        protected override ProductionBase[] PlayerProductionInfos()
            => PlayerManager.Data.History.AlbumList
                .OrderByDescending(album => album.Id)
                .ToArray();
    }
}