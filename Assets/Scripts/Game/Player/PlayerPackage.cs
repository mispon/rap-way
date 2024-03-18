using Core.OrderedStarter;

namespace Game.Player
{
    public partial class PlayerPackage : GamePackage<PlayerPackage>, IStarter
    {
        public void OnStart()
        {
            RegisterHandlers();
        }
    }
}