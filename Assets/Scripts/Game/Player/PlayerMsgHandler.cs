using Game.Player.Energy;
using Game.Player.Goods;
using Game.Player.Hype;
using Game.Player.State;
using Game.Player.Team;

namespace Game.Player
{
    public partial class PlayerPackage
    {
        private readonly HypeMessagesHandler _hypeHandler = new();
        private readonly TeamMessagesHandler _teamHandler = new();
        private readonly StateMessagesHandler _stateHandler = new();
        private readonly EnergyMessagesHandler _energyHandler = new();
        private readonly GoodsEventsHandler _goodsHandler = new();

        protected override void RegisterHandlers()
        {
            _hypeHandler.RegisterHandlers(disposable);
            _teamHandler.RegisterHandlers(disposable);
            _stateHandler.RegisterHandlers(disposable);
            _energyHandler.RegisterHandlers(disposable);
            _goodsHandler.RegisterHandlers(disposable);
        }
    }
}