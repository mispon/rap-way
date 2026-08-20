using RapWay.Core.Audio;
using RapWay.Core.Services;
using RapWay.Core.UI;
using VContainer;

namespace RapWay.App.Installers
{
    public static class CoreSystemInstaller
    {
        public static void InstallCoreSystems(this IContainerBuilder builder)
        {
            builder.Register<SceneLoaderService>(Lifetime.Singleton);
            builder.Register<AudioService>(Lifetime.Singleton);
        }
    }
}
