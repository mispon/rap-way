using VContainer;
using VContainer.Unity;

namespace RapWay.App.Scopes
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameStartup>();
        }
    }
}