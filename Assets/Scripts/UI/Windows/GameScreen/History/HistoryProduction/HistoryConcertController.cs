using System;
using System.Linq;
using Models.Production;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.History.HistoryProduction
{
    [Serializable]
    public class HistoryConcertController: HistoryProductionController
    {
        protected override ProductionBase[] PlayerProductionInfos()
            => PlayerAPI.Data.History.ConcertList
                .OrderByDescending(concert => concert.Id)
                .ToArray();
    }
}