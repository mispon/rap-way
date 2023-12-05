using Core;
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
        }

        private void OpenRappersTab()
        {
            if (rappersTab.IsOpen())
            {
                return;
            }
            
            SoundManager.Instance.PlaySwitch();
            UpdateTabsButtons(rappersTabButton, labelsTabButton);
            
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
            
            SoundManager.Instance.PlaySwitch();
            UpdateTabsButtons(labelsTabButton, rappersTabButton);
     
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