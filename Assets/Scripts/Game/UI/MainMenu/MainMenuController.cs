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
        
        [Header("Управление меню")]
        [SerializeField] private Button menuButton;
        [SerializeField] private Animator menuAnimator;
        [SerializeField] private string showMenuTriggerName;
        [SerializeField] private string hideMenuTriggerName;

        private bool _menuState;
        
        public void OnStart()
        {
            SetUpPlayerData();
            SetUpButtons();
        }

        /// <summary>
        /// Устанавливает метрики текущего сохранения
        /// </summary>
        private void SetUpPlayerData()
        {
            var playerInfo = GameManager.Instance.PlayerData;
            moneyText.text = playerInfo.Money.DisplayShort();
            fansText.text = playerInfo.Fans.DisplayShort();
            hypeText.text = playerInfo.Hype.ToString();
        }

        /// <summary>
        /// Создает обработчики нажатия на кнопки
        /// </summary>
        private void SetUpButtons()
        {
            newGameButton.onClick.AddListener(()=> ShowPanel(newPlayerPanel));
            continueGameButton.onClick.AddListener(()=> SceneManager.Instance.LoadGameScene());
            settingsButton.onClick.AddListener(()=> ShowPanel(settingsPanel));
            aboutButton.onClick.AddListener(()=> ShowPanel(aboutPanel));
            
            menuButton.onClick.AddListener(ChangeMenuState);
        }

        /// <summary>
        /// Открытие/закрытие меню
        /// </summary>
        private void ChangeMenuState()
        {
            menuAnimator.SetTrigger(_menuState ? hideMenuTriggerName : showMenuTriggerName);
            _menuState = !_menuState;
        }

        /// <summary>
        /// Переход к панели 
        /// </summary>
        private void ShowPanel(GameObject panel)
        {
            SetWindowActive(panel, true);
            _menuState = true;
            ChangeMenuState();
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

