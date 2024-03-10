using System;
using System.Linq;
using Models.Production;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.History.HistoryProduction
{
    [Serializable]
    public class HistoryTrackController: HistoryProductionController
    {
        protected override ProductionBase[] PlayerProductionInfos() => 
            PlayerAPI.Data.History.TrackList
                .OrderByDescending(track => track.Id)
                .ToArray();
    }
}