using RapWay.Composition.Unity;
using RapWay.Composition.Unity.Content;
using RapWay.Composition.Unity.Persistence;
using RapWay.Composition.Unity.Presentation;
using VContainer;
using VContainer.Unity;

namespace RapWay.App.Scopes
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.InstallSimulationMessaging();
            builder.InstallGameContent();
            builder.InstallSimulationPersistence();
            builder.RegisterEntryPoint<ActivityDemoBootstrapper>(Lifetime.Scoped);
            builder.RegisterEntryPoint<GameStartup>();
        }
    }
}
