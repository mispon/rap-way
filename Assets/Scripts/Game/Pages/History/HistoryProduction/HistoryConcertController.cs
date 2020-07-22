using System;
using System.Linq;
using Models.Info.Production;

namespace Game.Pages.History.HistoryProduction
{
    /// <summary>
    /// Контроллер UI-элементов. Обрабатывает команду на отображение информации по Концертам
    /// </summary>
    [Serializable]
    public class HistoryConcertController: HistoryProductionController
    {
        protected override Production[] PlayerProductionInfos()
            => PlayerManager.Data.History.ConcertList
                .OrderByDescending(concert => concert.Id)
                .ToArray();
    }
}