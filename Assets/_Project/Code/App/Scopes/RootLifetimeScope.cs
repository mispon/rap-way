using _Project.Data.Settings;
using RapWay.Core.Audio;
using RapWay.Core.SaveSystem.Services;
using RapWay.Core.Services;
using RapWay.Core.TimeSystem;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace RapWay.App.Scopes
{
    public class RootLifetimeScope : LifetimeScope
    {
        [Header("Settings")]
        [SerializeField] private TimeConfig timeConfig;
        
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterConfigs(builder);
            RegisterSaveSystem(builder);
            RegisterCoreServices(builder);
            RegisterGameServices(builder);
        }

        private void RegisterConfigs(IContainerBuilder builder)
        {
            builder.RegisterInstance(timeConfig);
        }
        
        private static void RegisterSaveSystem(IContainerBuilder builder)
        {
            builder.Register<FileStorageService>(Lifetime.Singleton).As<IStorageService>();
            builder.Register<SaveLoadService>(Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<AutoSaveManager>();
        }
        
        private static void RegisterCoreServices(IContainerBuilder builder)
        {
            builder
                .Register<MessageBroker>(Lifetime.Singleton)
                .As<IMessageBroker>();
            
            builder.Register<SceneLoaderService>(Lifetime.Singleton);
            builder.Register<AudioService>(Lifetime.Singleton);
        }
        
        private static void RegisterGameServices(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameTimeService>().AsSelf();
        }
    }
}