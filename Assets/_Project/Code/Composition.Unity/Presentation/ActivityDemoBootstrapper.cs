using System;
using RapWay.Application.Activities;
using RapWay.Core.Localization;
using RapWay.Presentation.Unity.Activities;
using RapWay.Presentation.Unity.Navigation;
using RapWay.Presentation.Unity.Shell;
using RapWay.Presentation.Unity.UiToolkit;
using UnityEngine;
using VContainer.Unity;

namespace RapWay.Composition.Unity.Presentation
{
    public sealed class ActivityDemoBootstrapper : IStartable
    {
        private readonly IActivityLoop _activityLoop;
        private readonly IActivityDefinitionCatalog _definitions;
        private readonly IGameLocalizationService _localizationService;
        private readonly IAppShellFlow _appShellFlow;
        private readonly IUiNavigator _uiNavigator;
        private readonly UiToolkitPresentationSettings _uiToolkitPresentationSettings;

        public ActivityDemoBootstrapper(
            IActivityLoop activityLoop,
            IActivityDefinitionCatalog definitions,
            IGameLocalizationService localizationService,
            IAppShellFlow appShellFlow,
            IUiNavigator uiNavigator,
            UiToolkitPresentationSettings uiToolkitPresentationSettings)
        {
            _activityLoop = activityLoop ?? throw new ArgumentNullException(nameof(activityLoop));
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _appShellFlow = appShellFlow ?? throw new ArgumentNullException(nameof(appShellFlow));
            _uiNavigator = uiNavigator ?? throw new ArgumentNullException(nameof(uiNavigator));
            _uiToolkitPresentationSettings = uiToolkitPresentationSettings ?? throw new ArgumentNullException(nameof(uiToolkitPresentationSettings));
        }

        public void Start()
        {
            GameObject demoObject = new("Activity Demo");
            demoObject.AddComponent<ActivityDemoPresenter>().Initialize(
                _activityLoop,
                _definitions,
                _localizationService,
                _appShellFlow,
                _uiNavigator,
                _uiToolkitPresentationSettings);
        }
    }
}
