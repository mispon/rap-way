using RapWay.Core.Audio;
using RapWay.Core.SaveSystem;
using RapWay.Core.Services;
using VContainer;
using VContainer.Unity;

namespace RapWay.App.Scopes
{
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Global services
            builder.Register<SceneLoaderService>(Lifetime.Singleton);
            builder.Register<SaveSystem>(Lifetime.Singleton);
            
            // Audio, Localization and etc.
            builder.Register<AudioManager>(Lifetime.Singleton);

            // Опционально: Точка входа для глобальной инициализации
            // Если нужно что-то сделать ПЕРЕД тем, как загрузится любая сцена
            // builder.RegisterEntryPoint<GlobalInitializer>();
        }
    }
}