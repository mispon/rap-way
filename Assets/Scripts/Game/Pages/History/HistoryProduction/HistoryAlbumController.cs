using System;
using System.Linq;
using Models.Info.Production;

namespace Game.Pages.History.HistoryProduction
{
    /// <summary>
    /// Контроллер UI-элементов. Обрабатывает команду на отображение информации по Альбомам
    /// </summary>
    [Serializable]
    public class HistoryAlbumController: HistoryProductionController
    {
        protected override Production[] PlayerProductionInfos()
            => PlayerManager.Data.History.AlbumList
                .OrderByDescending(album => album.Id)
                .ToArray();
    }
}