using System;
using System.Linq;
using Game.Player;
using Models.Production;

namespace UI.Windows.GameScreen.History.HistoryProduction
{
    [Serializable]
    public class HistoryTrackController: HistoryProductionController
    {
        protected override ProductionBase[] PlayerProductionInfos() => 
            PlayerManager.Data.History.TrackList
                .OrderByDescending(track => track.Id)
                .ToArray();
    }
}