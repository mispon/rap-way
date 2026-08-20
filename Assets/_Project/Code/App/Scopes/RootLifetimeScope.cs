using _Project.Data.Settings;
using RapWay.App.Installers;
using RapWay.Composition.Unity.Persistence;
using RapWay.Composition.Unity.Presentation;
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
            
            builder.InstallCoreSystems();
            builder.InstallGameSystems();
            builder.InstallPersistentSaveStore(UnityEngine.Application.persistentDataPath);
            builder.InstallStage4Presentation();
        }

        private void RegisterConfigs(IContainerBuilder builder)
        {
            builder.RegisterInstance(timeConfig);
        }
    }
}
