using Core;
using Models.Player;
using UnityEngine;
using UnityEngine.UI;
using EventType = Core.EventType;

namespace Game.UI.GameScreen
{
    /// <summary>
    /// Контроллер главного окна игры
    /// </summary>
    public class GameScreenController: MonoBehaviour
    {
        [Header("HUD контроллы")]
        [SerializeField] private Text playerNickname;
        [SerializeField] private Text playerFans;
        [SerializeField] private Text playerMoney;
        [SerializeField] private Text playerHype;
        [SerializeField] private Text currentDate;

        [Header("Группа основных действий")]
        [SerializeField] private Button productionFoldoutButton;
        [SerializeField] private Animation foldoutAnimation;
        [SerializeField] private string foldoutShowAnim;
        [SerializeField] private string foldoutHideAnim;

        [Space, SerializeField] private Button mainMenuButton;

        private bool _productionShown;
        
        private void Start()
        {
            productionFoldoutButton.onClick.AddListener(OnProductionClick);
            mainMenuButton.onClick.AddListener(OnMainMenuClick);
            
            EventManager.AddHandler(EventType.GameReady, OnGameReady);
        }

        /// <summary>
        /// Инициализирует интерфейса игрока 
        /// </summary>
        private void InitHUD()
        {
            PlayerManager.Instance.SetHUD(this);
        }

        /// <summary>
        /// Обновляет интерфейс игрока
        /// </summary>
        public void UpdateHUD(PlayerData playerData)
        {
            playerNickname.text = playerData.Info.NickName;
            playerMoney.text = playerData.Data.GetMoney();
            playerFans.text = playerData.Data.GetFans();
            playerHype.text = playerData.Data.Hype.ToString();
            currentDate.text = TimeManager.Instance.DisplayNow;
        }

        /// <summary>
        /// Обработчик истечения дня
        /// </summary>
        private void OnDayLeft()
        {
            currentDate.text = TimeManager.Instance.DisplayNow;
        }

        /// <summary>
        /// Переключение выпадающего списка основных действий
        /// </summary>
        private void OnProductionClick()
        {
            _productionShown = !_productionShown;
            foldoutAnimation.Play(_productionShown ? foldoutShowAnim : foldoutHideAnim);
        }

        /// <summary>
        /// Выход в главное меню
        /// </summary>
        private static void OnMainMenuClick()
        {
            GameManager.Instance.SaveApplicationData();
            SceneManager.Instance.LoadMainScene();
        }

        #region GAME EVENTS

        private void OnGameReady(object[] args)
        {
            InitHUD();
            TimeManager.Instance.onDayLeft += OnDayLeft;
        }

        private void OnDisable()
        {
            TimeManager.Instance.onDayLeft -= OnDayLeft;
            EventManager.RemoveHandler(EventType.GameReady, OnGameReady);
        }

        #endregion
    }
}