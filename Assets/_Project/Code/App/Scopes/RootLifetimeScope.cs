using _Project.Data.Settings;
using RapWay.App.Installers;
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
            
            builder.InstallSaveSystem();
            builder.InstallCoreSystems();
            builder.InstallGameSystems();
        }

        private void RegisterConfigs(IContainerBuilder builder)
        {
            builder.RegisterInstance(timeConfig);
        }
    }
}