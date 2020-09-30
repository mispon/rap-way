using Core;
using Core.Interfaces;
using Models.UI;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.Extensions;

namespace Game.UI.MainMenu
{
    /// <summary>
    /// Контроллер управления главным меню
    /// </summary>
    public class MainMenuController : Singleton<MainMenuController>, IStarter
    {
        [Header("Metrics UI")] 
        [SerializeField] private Text moneyText;
        [SerializeField] private Text fansText;
        [SerializeField] private Text hypeText;

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

        /// <summary>
        /// Набор кноппок открытия панелей
        /// </summary>
        private Button[] MenuButtons => new[]
        {
            newGameButton,
            continueGameButton,
            settingsButton,
            aboutButton
        };
        
        public void OnStart()
        {
            SetUpButtons();
        }
        
        /// <summary>
        /// Создает обработчики нажатия на кнопки
        /// </summary>
        private void SetUpButtons()
        {
            foreach (var button in MenuButtons)
                button.onClick.AddListener(SoundManager.Instance.PlayClick);
            
            newGameButton.onClick.AddListener(()=> ShowPanel(newPlayerPanel));
            continueGameButton.onClick.AddListener(SceneManager.Instance.LoadGameScene);
            settingsButton.onClick.AddListener(()=> ShowPanel(settingsPanel));
            aboutButton.onClick.AddListener(()=> ShowPanel(aboutPanel));
        }

        /// <summary>
        /// Переход к панели 
        /// </summary>
        private void ShowPanel(GameObject panel)
        {
            SetWindowActive(panel, true);
        }
        
        /// <summary>
        /// Открытие/закрытие окон главного меню
        /// </summary>
        public static void SetWindowActive(GameObject panel, bool value)
        {
            panel.SetActive(value);
            Instance.menuCanvasGroupController.SetActive(!value);
        }
    }    
}

