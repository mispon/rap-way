using System;
using System.Linq;
using Game.Player;
using Models.Production;

namespace UI.Windows.Pages.History.HistoryProduction
{
    /// <summary>
    /// Контроллер UI-элементов. Обрабатывает команду на отображение информации по Альбомам
    /// </summary>
    [Serializable]
    public class HistoryAlbumController: HistoryProductionController
    {
        protected override ProductionBase[] PlayerProductionInfos()
            => PlayerManager.Data.History.AlbumList
                .OrderByDescending(album => album.Id)
                .ToArray();
    }
}