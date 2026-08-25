using RapWay.Application.Activities;
using RapWay.Application.Persistence;
using RapWay.Infrastructure.Persistence;
using VContainer;
using VContainer.Unity;

namespace RapWay.Composition.Unity.Persistence
{
    public static class SimulationPersistenceInstaller
    {
        public static void InstallPersistentSaveStore(this IContainerBuilder builder, string saveDirectoryPath)
        {
            builder.RegisterInstance<IGameSaveStore>(new JsonGameSaveStore(saveDirectoryPath));
        }

        public static void InstallSimulationPersistence(this IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameSessionCoordinator>(Lifetime.Scoped)
                .AsSelf()
                .As<IGameStateSnapshotSource>()
                .As<IActivityLoop>();
            builder.RegisterEntryPoint<CommittedEventAutosaveScheduler>(Lifetime.Scoped);
            builder.RegisterComponentOnNewGameObject<GameSaveLifecycleAdapter>(
                Lifetime.Scoped,
                "Game Save Lifecycle");
            builder.RegisterBuildCallback(container => container.Resolve<GameSaveLifecycleAdapter>());
        }
    }
}
