using System;
using System.Linq;
using Game.Player;
using Models.Production;

namespace UI.Windows.Pages.History.HistoryProduction
{
    /// <summary>
    /// Контроллер UI-элементов. Обрабатывает команду на отображение информации по Трекам
    /// </summary>
    [Serializable]
    public class HistoryTrackController: HistoryProductionController
    {
        protected override ProductionBase[] PlayerProductionInfos() => 
            PlayerManager.Data.History.TrackList
                .OrderByDescending(track => track.Id)
                .ToArray();
    }
}