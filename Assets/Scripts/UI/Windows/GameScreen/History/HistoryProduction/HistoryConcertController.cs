using System;
using System.Linq;
using Game.Player;
using Models.Production;

namespace UI.Windows.GameScreen.History.HistoryProduction
{
    [Serializable]
    public class HistoryConcertController: HistoryProductionController
    {
        protected override ProductionBase[] PlayerProductionInfos()
            => PlayerManager.Data.History.ConcertList
                .OrderByDescending(concert => concert.Id)
                .ToArray();
    }
}