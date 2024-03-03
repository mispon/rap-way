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

        public override void Show(object ctx = null)
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
            
            base.Show(ctx);
        }

        public void ShowControls()
        {
            foreach (var go in pageControls)
            {
                go.SetActive(true);
            }
        }
        
        public void HideControls()
        {
            foreach (var go in pageControls)
            {
                go.SetActive(false);
            }
        }

        private void OpenRappersTab()
        {
            if (rappersTab.IsActive())
                return;
            
            UpdateTabsButtons(rappersTabButton, labelsTabButton);
            if (!_isFirstOpen) 
                SoundManager.Instance.PlaySound(UIActionType.Switcher);
            
            newRappersPage.Hide();
            labelsTab.Hide();
            rappersTab.Show();
        }

        private void OpenLabelsTab()
        {
            if (labelsTab.IsActive())
                return;
            
            UpdateTabsButtons(labelsTabButton, rappersTabButton);
            SoundManager.Instance.PlaySound(UIActionType.Switcher);
     
            newRappersPage.Hide();
            rappersTab.Hide();
            labelsTab.Show();
        }

        private void UpdateTabsButtons(Button active, Button inactive)
        {
            active.image.color = activeTabColor;
            inactive.image.color = inactiveTabColor;
        }
        
        protected override void AfterShow()
        {
            ShowControls();
            
            base.AfterShow();
            OpenRappersTab();

            _isFirstOpen = false;
        }

        protected override void AfterHide()
        {
            _isFirstOpen = true;
        }
    }
}