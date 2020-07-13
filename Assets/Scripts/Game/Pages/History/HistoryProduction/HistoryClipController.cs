using System;
using System.Linq;
using Models.Info.Production;

namespace Game.Pages.History.HistoryProduction
{
    /// <summary>
    /// Контроллер UI-элементов. Обрабатывает команду на отображение информации по Клипам
    /// </summary>
    [Serializable]
    public class HistoryClipController: HistoryProductionController
    {
        protected override Production[] PlayerProductionInfos()
            => PlayerManager.Data.History.ClipList
                .OrderByDescending(cl=> cl.Id)
                .ToArray();
    }
}