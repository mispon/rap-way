using Core;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Charts
{
    public class ChartsPage : Page
    {
        [SerializeField] private GameObject[] pageControls;
        [Space]
        [SerializeField] private Button rappersTabButton;
        [SerializeField] private Button labelsTabButton;
        [Space]
        [SerializeField] private Page rappersTab;
        [SerializeField] private Page newRappersPage;
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

        public void Show()
        {
            foreach (var go in pageControls)
            {
                go.SetActive(true);
            }
        }
        
        public void Hide()
        {
            foreach (var go in pageControls)
            {
                go.SetActive(false);
            }
        }

        protected override void AfterPageOpen()
        {
            Show();
            base.AfterPageOpen();
            OpenRappersTab();

            _isFirstOpen = false;
        }

        protected override void AfterPageClose()
        {
            _isFirstOpen = true;
        }

        private void OpenRappersTab()
        {
            if (rappersTab.IsOpen())
            {
                return;
            }
            
            
            UpdateTabsButtons(rappersTabButton, labelsTabButton);
            if (!_isFirstOpen) 
                SoundManager.Instance.PlaySound(UIActionType.Switcher);
            
            newRappersPage.Close();
            labelsTab.Close();
            rappersTab.Open();
        }

        private void OpenLabelsTab()
        {
            if (labelsTab.IsOpen())
            {
                return;
            }
            
            UpdateTabsButtons(labelsTabButton, rappersTabButton);
            SoundManager.Instance.PlaySound(UIActionType.Switcher);
     
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