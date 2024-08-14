using Core;
using Core.Context;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Charts
{
    public enum ChartsTabType
    {
        Rappers,
        Labels
    }

    public class ChartsPage : Page
    {
        [SerializeField] private Button rappersTabButton;
        [SerializeField] private Button labelsTabButton;

        [Space]
        [SerializeField] private Page rappersTab;
        [SerializeField] private Page labelsTab;

        [Space]
        [SerializeField] private Color activeTabColor;
        [SerializeField] private Color inactiveTabColor;

        private bool _isFirstOpen = true;

        private void Start()
        {
            rappersTabButton.onClick.AddListener(OpenRappersTab);
            labelsTabButton.onClick.AddListener(OpenLabelsTab);
        }

        private void OpenRappersTab()
        {
            if (rappersTab.IsActive())
                return;

            UpdateTabsButtons(rappersTabButton, labelsTabButton);
            if (!_isFirstOpen)
                SoundManager.Instance.PlaySound(UIActionType.Switcher);

            labelsTab.Hide();
            rappersTab.Show();
        }

        private void OpenLabelsTab()
        {
            if (labelsTab.IsActive())
                return;

            UpdateTabsButtons(labelsTabButton, rappersTabButton);
            SoundManager.Instance.PlaySound(UIActionType.Switcher);

            rappersTab.Hide();
            labelsTab.Show();
        }

        private void UpdateTabsButtons(Button active, Button inactive)
        {
            active.image.color = activeTabColor;
            inactive.image.color = inactiveTabColor;
        }

        protected override void AfterShow(object ctx = null)
        {
            var tab = ctx.Value<ChartsTabType>();
            switch (tab)
            {
                case ChartsTabType.Labels:
                    OpenLabelsTab();
                    break;

                case ChartsTabType.Rappers:
                default:
                    OpenRappersTab();
                    break;
            }

            _isFirstOpen = false;
        }

        protected override void AfterHide()
        {
            _isFirstOpen = true;
        }
    }
}