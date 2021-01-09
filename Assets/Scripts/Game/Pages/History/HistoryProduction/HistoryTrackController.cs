using System;
using System.Linq;
using Models.Info.Production;

namespace Game.Pages.History.HistoryProduction
{
    /// <summary>
    /// Контроллер UI-элементов. Обрабатывает команду на отображение информации по Трекам
    /// </summary>
    [Serializable]
    public class HistoryTrackController: HistoryProductionController
    {
        protected override Production[] PlayerProductionInfos() => 
            PlayerManager.Data.History.TrackList
                .OrderByDescending(track => track.Id)
                .ToArray();
    }
}