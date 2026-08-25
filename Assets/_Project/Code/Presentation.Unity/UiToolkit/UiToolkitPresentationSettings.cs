using System;
using RapWay.Presentation.Unity.Navigation;
using UnityEngine;
using UnityEngine.UIElements;

namespace RapWay.Presentation.Unity.UiToolkit
{
    [CreateAssetMenu(
        fileName = "UiToolkitPresentationSettings",
        menuName = "Rap Way/Presentation/UI Toolkit Settings")]
    public sealed class UiToolkitPresentationSettings : ScriptableObject
    {
        [SerializeField] private PanelSettings panelSettings;
        [SerializeField] private VisualTreeAsset appShellVisualTree;
        [SerializeField] private VisualTreeAsset activityDemoVisualTree;
        [SerializeField, Min(1)] private int navigationHistoryLimit = UiNavigationDefaults.HistoryLimit;

        public PanelSettings PanelSettings => panelSettings;

        public VisualTreeAsset AppShellVisualTree => appShellVisualTree;

        public VisualTreeAsset ActivityDemoVisualTree => activityDemoVisualTree;

        public int NavigationHistoryLimit => navigationHistoryLimit;

        public void EnsureValid()
        {
            if (panelSettings == null ||
                panelSettings.themeStyleSheet == null ||
                appShellVisualTree == null ||
                activityDemoVisualTree == null)
            {
                throw new InvalidOperationException(
                    $"UI Toolkit presentation settings '{name}' are incomplete. Assign every UI reference in the Inspector.");
            }

            if (navigationHistoryLimit < 1)
            {
                throw new InvalidOperationException(
                    $"UI Toolkit presentation settings '{name}' require a navigation history limit of at least one.");
            }
        }
    }
}
