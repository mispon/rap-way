using System;
using Core;
using Core.Interfaces;
using Enums;
using Models.Player;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.Extensions;

namespace Game.UI.GameScreen
{
    /// <summary>
    /// Контроллер главного окна игры
    /// </summary>
    public class GameScreenController: Singleton<GameScreenController>, IStarter
    {
        [Header("HUD контроллы")]
        [SerializeField] private Image playerAvatar;
        [SerializeField] private Text playerNickname;
        [SerializeField] private Text playerFans;
        [SerializeField] private Text playerMoney;
        [SerializeField] private Text playerHype;
        [SerializeField] private Text currentDate;
        [Space]
        [SerializeField] private Button moneyButton;
        [SerializeField] private Button fansButton;
        [SerializeField] private Button hypeButton;
        [Space]
        [SerializeField] private StatDescItem[] statDescItems;
        [SerializeField] private StatsDescriptionPage statsDescPage;

        [Header("Иконки аватара")]
        [SerializeField] private Sprite maleIcon;
        [SerializeField] private Sprite femaleIcon;
        
        [Header("Группа основных действий")]
        [SerializeField] private Button productionFoldoutButton;
        [SerializeField] private Animation foldoutAnimation;
        [SerializeField] private string foldoutShowAnim;
        [SerializeField] private string foldoutHideAnim;

        [Space, SerializeField] private Button mainMenuButton;

        private bool _productionShown;
        
        public void OnStart()
        {
            moneyButton.onClick.AddListener(() => ShowDescriptionPage(statDescItems[0]));
            fansButton.onClick.AddListener(() => ShowDescriptionPage(statDescItems[1]));
            hypeButton.onClick.AddListener(() => ShowDescriptionPage(statDescItems[2]));

            productionFoldoutButton.onClick.AddListener(OnProductionClick);
            mainMenuButton.onClick.AddListener(OnMainMenuClick);
            TimeManager.Instance.onDayLeft += OnDayLeft;
        }

        /// <summary>
        /// Открывает страницу с описанием основной характеристики
        /// </summary>
        private void ShowDescriptionPage(StatDescItem item)
        {
            SoundManager.Instance.PlayClick();
            statsDescPage.Show(item.Icon, item.NameKey, item.DescKey);
        }

        /// <summary>
        /// Обновляет интерфейс игрока
        /// </summary>
        public void UpdateHUD(PlayerData playerData)
        {
            playerAvatar.sprite = PlayerManager.Data.Info.Gender == Gender.Male
                ? maleIcon
                : femaleIcon;
            playerNickname.text = playerData.Info.NickName.ToUpper();
            playerMoney.text = $"{playerData.Money.GetMoney()}";
            playerFans.text = playerData.Fans.GetDisplay();
            playerHype.text = playerData.Hype.ToString();
            currentDate.text = TimeManager.Instance.DisplayNow;
        }

        public void SetVisibility(bool state)
        {
            gameObject.SetActive(state);
        }

        /// <summary>
        /// Скрытие выпадающего списка основных действий
        /// </summary>
        public void HideProductionGroup()
        {
            _productionShown = false;
            foldoutAnimation.Play(foldoutHideAnim);
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
            SoundManager.Instance.PlayClick();
            _productionShown = !_productionShown;
            foldoutAnimation.Play(_productionShown ? foldoutShowAnim : foldoutHideAnim);
        }

        /// <summary>
        /// Выход в главное меню
        /// </summary>
        private static void OnMainMenuClick()
        {
            SoundManager.Instance.PlayClick();
            GameManager.Instance.SaveApplicationData();
            SceneManager.Instance.LoadMainScene();
        }
        
        private void OnDestroy()
        {
            TimeManager.Instance.onDayLeft -= OnDayLeft;
        }
    }

    [Serializable]
    public class StatDescItem
    {
        public Sprite Icon;
        public string NameKey;
        public string DescKey;
    }
}