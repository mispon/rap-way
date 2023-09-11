using Core;
using Core.Interfaces;
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

        [Header("Окна")] 
        [SerializeField] private GameObject newPlayerPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject aboutPanel;
        
        [Header("CanvasGroup")] 
        [SerializeField] private CanvasGroupController menuCanvasGroupController;

        public void OnStart()
        {
            AppodealManager.Instance.ShowInterstitial();
            SetupButtons();
        }
        
        /// <summary>
        /// Создает обработчики нажатия на кнопки
        /// </summary>
        private void SetupButtons()
        {
            continueGameButton.interactable = GameManager.Instance.HasCharacter();

            newGameButton.onClick.AddListener(()=> ShowPanel(newPlayerPanel));
            continueGameButton.onClick.AddListener(ContinueGame);
            settingsButton.onClick.AddListener(()=> ShowPanel(settingsPanel));
            aboutButton.onClick.AddListener(()=> ShowPanel(aboutPanel));
        }

        /// <summary>
        /// Загружает игровую сцену
        /// </summary>
        private static void ContinueGame()
        {
            SoundManager.Instance.PlayClick();
            SceneManager.Instance.LoadGameScene();
        }

        /// <summary>
        /// Переход к панели 
        /// </summary>
        private static void ShowPanel(GameObject panel)
        {
            SoundManager.Instance.PlayClick();
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