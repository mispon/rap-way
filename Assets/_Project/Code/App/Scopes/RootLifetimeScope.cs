using _Project.Data.Settings;
using RapWay.App.Installers;
using RapWay.Data;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace RapWay.App.Scopes
{
    public class RootLifetimeScope : LifetimeScope
    {
        [Header("Settings")]
        [SerializeField] private TimeConfig timeConfig;
        [Header("UI")]
        [SerializeField] private UIConfig uiConfig;
        
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterConfigs(builder);
            
            builder.InstallSaveSystem();
            builder.InstallCoreSystems();
            builder.InstallGameSystems();
            builder.InstallUISystems(uiConfig);
        }

        private void RegisterConfigs(IContainerBuilder builder)
        {
            builder.RegisterInstance(timeConfig);
        }
    }
}