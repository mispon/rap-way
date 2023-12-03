using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Charts
{
    public class ChartsPage : Page
    {
        [SerializeField] private Button rappersTabButton;
        [SerializeField] private Button labelsTabButton;
        [Space]
        [SerializeField] private Page rappersTab;
        [SerializeField] private Page newRappersPage;
        [SerializeField] private Page labelsTab;
        [Space]
        [SerializeField] private Color activeTabColor;
        [SerializeField] private Color inactiveTabColor;
        
        private bool _isRappersTabOpened;
        
        private void Start()
        {
            rappersTabButton.onClick.AddListener(OpenRappersTab);
            labelsTabButton.onClick.AddListener(OpenLabelsTab);
        }

        protected override void AfterPageOpen()
        {
            base.AfterPageOpen();
            OpenRappersTab();
        }

        private void OpenRappersTab()
        {
            if (_isRappersTabOpened) {
                return;
            }

            UpdateTabsButtons(rappersTabButton, labelsTabButton);
            _isRappersTabOpened = true;
            
            newRappersPage.Close();
            labelsTab.Close();
            rappersTab.Open();
        }

        private void OpenLabelsTab()
        {
            if (!_isRappersTabOpened) {
                return;
            }

            UpdateTabsButtons(labelsTabButton, rappersTabButton);
            _isRappersTabOpened = false;
            
            newRappersPage.Close();
            rappersTab.Close();
            labelsTab.Open();
        }

        private void UpdateTabsButtons(Button active, Button inactive)
        {
            active.image.color = activeTabColor;
            inactive.image.color = inactiveTabColor;
        }
    }
}