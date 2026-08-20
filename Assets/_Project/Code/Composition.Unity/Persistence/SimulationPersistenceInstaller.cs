using RapWay.Application.Persistence;
using RapWay.Infrastructure.Persistence;
using VContainer;
using VContainer.Unity;

namespace RapWay.Composition.Unity.Persistence
{
    public static class SimulationPersistenceInstaller
    {
        public static void InstallSimulationPersistence(this IContainerBuilder builder, string saveDirectoryPath)
        {
            builder.RegisterInstance<IGameSaveStore>(new JsonGameSaveStore(saveDirectoryPath));
            builder.RegisterEntryPoint<GameSessionCoordinator>(Lifetime.Scoped)
                .AsSelf()
                .As<IGameStateSnapshotSource>();
            builder.RegisterEntryPoint<CommittedEventAutosaveScheduler>(Lifetime.Scoped);
            builder.RegisterComponentOnNewGameObject<GameSaveLifecycleAdapter>(
                Lifetime.Scoped,
                "Game Save Lifecycle");
            builder.RegisterBuildCallback(container => container.Resolve<GameSaveLifecycleAdapter>());
        }
    }
}
