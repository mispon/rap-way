using System;
using System.Linq;
using Game.Player;
using Models.Production;

namespace UI.Windows.GameScreen.History.HistoryProduction
{
    [Serializable]
    public class HistoryClipController: HistoryProductionController
    {
        protected override ProductionBase[] PlayerProductionInfos()
            => PlayerManager.Data.History.ClipList
                .OrderByDescending(clip => clip.Id)
                .ToArray();
    }
}