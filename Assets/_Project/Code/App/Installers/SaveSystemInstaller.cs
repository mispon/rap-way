using RapWay.Core.SaveSystem.Services;
using VContainer;
using VContainer.Unity;

namespace RapWay.App.Installers
{
    public static class SaveSystemInstaller
    {
        public static void InstallSaveSystem(this IContainerBuilder builder)
        {
            builder.Register<FileStorageService>(Lifetime.Singleton).As<IStorageService>();
            builder.Register<SaveLoadService>(Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<AutoSaveManager>();
        }
    }
}