using RapWay.Core.TimeSystem;
using VContainer;
using VContainer.Unity;

namespace RapWay.App.Installers
{
    public static class GameSystemInstaller
    {
        public static void InstallGameSystems(this IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameTimeService>().AsSelf();
        }
    }
}