using System;
using System.Linq;
using Game.Player;
using Models.Production;

namespace UI.Windows.Pages.History.HistoryProduction
{
    /// <summary>
    /// Контроллер UI-элементов. Обрабатывает команду на отображение информации по Клипам
    /// </summary>
    [Serializable]
    public class HistoryClipController: HistoryProductionController
    {
        protected override ProductionBase[] PlayerProductionInfos()
            => PlayerManager.Data.History.ClipList
                .OrderByDescending(clip => clip.Id)
                .ToArray();
    }
}