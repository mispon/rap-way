using System;
using System.Linq;
using Models.Production;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.History.HistoryProduction
{
    [Serializable]
    public class HistoryClipController: HistoryProductionController
    {
        protected override ProductionBase[] PlayerProductionInfos()
            => PlayerAPI.Data.History.ClipList
                .OrderByDescending(clip => clip.Id)
                .ToArray();
    }
}