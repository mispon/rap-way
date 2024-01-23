using Core;
using Core.Interfaces;
using Data;
using Firebase.Analytics;
using Game.Pages.AskReview;
using Models.UI;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.UI.MainMenu
{
    /// <summary>
    /// Контроллер управления главным меню
    /// </summary>
    public class MainMenuController : Singleton<MainMenuController>, IStarter
    {
        [Header("Кнопки меню")] 
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueGameButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button aboutButton;
        [SerializeField] private Button exitButton;

        [Header("Окна")] 
        [SerializeField] private GameObject newPlayerPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject aboutPanel;
        [Space]
        [SerializeField] private AskReviewPage reviewPage;
        [SerializeField] private ProductionAnim anim;
        
        [Header("CanvasGroup")] 
        [SerializeField] private CanvasGroupController menuCanvasGroupController;

        public void OnStart()
        {
            anim.Refresh();
            SetupButtons();
            
            if (!GameManager.Instance.HasAnySaves())
            {
                FirebaseAnalytics.LogEvent(FirebaseGameEvents.GameFirstOpen);
            }
            
            if (!GameManager.Instance.GameStats.AskedReview && GameManager.Instance.PlayerData.Fans > 0)
            {
                reviewPage.Open();
            }
        }
        
        /// <summary>
        /// Setups buttons
        /// </summary>
        private void SetupButtons()
        {
            continueGameButton.interactable = GameManager.Instance.HasCharacter();

            newGameButton.onClick.AddListener(() =>
            {
                FirebaseAnalytics.LogEvent(FirebaseGameEvents.NewGamePage);
                ShowPanel(newPlayerPanel);
            });
            
            settingsButton.onClick.AddListener(() => ShowPanel(settingsPanel));
            aboutButton.onClick.AddListener(() => ShowPanel(aboutPanel));
            exitButton.onClick.AddListener(ExitGame);
        }

        /// <summary>
        /// Closes the game
        /// </summary>
        private static void ExitGame()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            GameManager.Instance.SaveApplicationData();
            Application.Quit();
        }

        /// <summary>
        /// Переход к панели 
        /// </summary>
        private static void ShowPanel(GameObject panel)
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            SetPanelActivity(panel, true);
        }
        
        /// <summary>
        /// Открытие/закрытие окон главного меню
        /// </summary>
        public static void SetPanelActivity(GameObject panel, bool value)
        {
            panel.SetActive(value);
            Instance.menuCanvasGroupController.SetActive(!value);
        }
    }    
}