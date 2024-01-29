using System;
using System.Linq;
using Game.Player;
using Models.Production;

namespace UI.Windows.Pages.History.HistoryProduction
{
    /// <summary>
    /// Контроллер UI-элементов. Обрабатывает команду на отображение информации по Концертам
    /// </summary>
    [Serializable]
    public class HistoryConcertController: HistoryProductionController
    {
        protected override ProductionBase[] PlayerProductionInfos()
            => PlayerManager.Data.History.ConcertList
                .OrderByDescending(concert => concert.Id)
                .ToArray();
    }
}