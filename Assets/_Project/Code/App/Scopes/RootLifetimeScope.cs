using RapWay.Core.Audio;
using RapWay.Core.SaveSystem.Services;
using RapWay.Core.Services;
using VContainer;
using VContainer.Unity;

namespace RapWay.App.Scopes
{
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Save system
            builder.Register<FileStorageService>(Lifetime.Singleton).As<IStorageService>();
            builder.Register<SaveLoadService>(Lifetime.Singleton);
            
            // Global systems
            builder.Register<SceneLoaderService>(Lifetime.Singleton);
            builder.Register<AudioManager>(Lifetime.Singleton);
            
            builder.RegisterComponentInHierarchy<AutoSaveManager>();
        }
    }
}