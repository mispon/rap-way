using Core;
using Core.Interfaces;
using Models.Player;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.UI.GameScreen
{
    /// <summary>
    /// Контроллер главного окна игры
    /// </summary>
    public class GameScreenController: MonoBehaviour, IStarter
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
        
        public void OnStart()
        {
            productionFoldoutButton.onClick.AddListener(OnProductionClick);
            mainMenuButton.onClick.AddListener(OnMainMenuClick);
            TimeManager.Instance.onDayLeft += OnDayLeft;
        }

        /// <summary>
        /// Обновляет интерфейс игрока
        /// </summary>
        public void UpdateHUD(PlayerData playerData)
        {
            playerNickname.text = playerData.Info.NickName;
            playerMoney.text = playerData.Money.GetDescription();
            playerFans.text = playerData.Fans.GetDescription();
            playerHype.text = playerData.Hype.ToString();
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
        
        private void OnDestroy()
        {
            TimeManager.Instance.onDayLeft -= OnDayLeft;
        }
    }
}