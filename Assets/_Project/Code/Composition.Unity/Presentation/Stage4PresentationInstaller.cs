using RapWay.Application.Navigation;
using RapWay.Application.Persistence;
using RapWay.Application.Session;
using RapWay.Composition.Unity.Navigation;
using RapWay.Core.Localization;
using RapWay.Infrastructure.Persistence;
using RapWay.Presentation.Unity.Shell;
using RapWay.Presentation.Unity.Navigation;
using RapWay.Presentation.Unity.UiToolkit;
using UnityEngine;
using VContainer;

namespace RapWay.Composition.Unity.Presentation
{
    public static class Stage4PresentationInstaller
    {
        private const string UiToolkitPresentationSettingsResourcePath = "Presentation/UiToolkitPresentationSettings";

        public static void InstallStage4Presentation(this IContainerBuilder builder)
        {
            UiToolkitPresentationSettings uiToolkitPresentationSettings = Resources.Load<UiToolkitPresentationSettings>(
                UiToolkitPresentationSettingsResourcePath);
            if (uiToolkitPresentationSettings == null)
            {
                throw new System.InvalidOperationException(
                    $"UI Toolkit presentation settings '{UiToolkitPresentationSettingsResourcePath}' were not found.");
            }

            uiToolkitPresentationSettings.EnsureValid();

            UiRouteRegistry uiRouteRegistry = UiRouteRegistry.CreateDefault();

            builder.Register<ISceneNavigator, UnitySceneNavigator>(Lifetime.Singleton);
            builder.Register<IGameSessionLaunchRequest, GameSessionLaunchRequest>(Lifetime.Singleton);
            builder.Register<ICareerSaveAvailabilityProbe, CareerSaveAvailabilityProbe>(Lifetime.Singleton);
            builder.Register<IGameLocalizationService, GameLocalizationService>(Lifetime.Singleton);
            builder.RegisterInstance(uiToolkitPresentationSettings);
            builder.RegisterInstance(uiRouteRegistry);
            builder.RegisterInstance<IUiNavigator>(new UiNavigator(
                uiRouteRegistry,
                uiToolkitPresentationSettings.NavigationHistoryLimit));
            builder.Register<IAppShellFlow, AppShellController>(Lifetime.Singleton);
        }
    }
}
