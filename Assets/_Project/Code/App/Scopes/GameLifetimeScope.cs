using RapWay.Composition.Unity;
using RapWay.Composition.Unity.Persistence;
using VContainer;
using VContainer.Unity;

namespace RapWay.App.Scopes
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.InstallSimulationMessaging();
            builder.InstallSimulationPersistence(UnityEngine.Application.persistentDataPath);
            builder.RegisterEntryPoint<GameStartup>();
        }
    }
}
