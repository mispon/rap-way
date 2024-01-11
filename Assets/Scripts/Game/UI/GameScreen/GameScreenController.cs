using System;
using Core;
using Core.Interfaces;
using Data;
using Enums;
using Models.Player;
using Scenes;
using MessageBroker.Messages.State;
using UniRx;
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
        private readonly CompositeDisposable _disposable = new();
        
        [Header("HUD контроллы")]
        [SerializeField] private Image playerAvatar;
        [SerializeField] private Text playerNickname;
        [SerializeField] private Button playerButton;
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

        [Space]
        [SerializeField] private Button mainMenuButton;

        [Space]
        [SerializeField] private GameObject personalPageHint;

        private IMessageBroker _messageBroker;
        private bool _productionShown;
        
        public void OnStart()
        {
            _messageBroker = GameManager.Instance.MessageBroker;
            
            moneyButton.onClick.AddListener(() => ShowDescriptionPage(statDescItems[0]));
            fansButton.onClick.AddListener(() => ShowDescriptionPage(statDescItems[1]));
            hypeButton.onClick.AddListener(() => ShowDescriptionPage(statDescItems[2]));
            playerButton.onClick.AddListener(() => personalPageHint.SetActive(false));

            productionFoldoutButton.onClick.AddListener(OnProductionClick);
            mainMenuButton.onClick.AddListener(OnMainMenuClick);
            TimeManager.Instance.onDayLeft += OnDayLeft;

            if (!GameManager.Instance.ShowedHints.Contains("personal_page_hint"))
            {
                personalPageHint.SetActive(true);
                GameManager.Instance.ShowedHints.Add("personal_page_hint");
            }
            
            HandleStateEvents();
        }

        /// <summary>
        /// Открывает страницу с описанием основной характеристики
        /// </summary>
        private void ShowDescriptionPage(StatDescItem item)
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            statsDescPage.Show(item.Icon, item.NameKey, item.DescKey);
        }

        /// <summary>
        /// Handles player's state updates
        /// </summary>
        private void HandleStateEvents()
        {
            _messageBroker
                .Receive<MoneyChangedEvent>()
                .Subscribe(e => playerMoney.text = e.NewVal.GetMoney())
                .AddTo(_disposable);
            _messageBroker
                .Receive<FansChangedEvent>()
                .Subscribe(e => playerFans.text = e.NewVal.GetDisplay())
                .AddTo(_disposable);
            _messageBroker
                .Receive<HypeChangedEvent>()
                .Subscribe(e => playerHype.text = e.NewVal.ToString())
                .AddTo(_disposable);
            _messageBroker
                .Receive<FullStateResponse>()
                .Subscribe(UpdateHUD)
                .AddTo(_disposable);
            
            _messageBroker.Publish(new FullStateRequest());
        }
        
        /// <summary>
        /// Updates main HUD
        /// </summary>
        private void UpdateHUD(FullStateResponse resp)
        {
            playerNickname.text = resp.NickName.ToUpper();
            playerAvatar.sprite = resp.Gender == Gender.Male ? maleIcon : femaleIcon;
            playerMoney.text = resp.Money.GetMoney();
            playerFans.text = resp.Fans.GetDisplay();
            playerHype.text = resp.Hype.ToString();
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
            SoundManager.Instance.PlaySound(UIActionType.Click);
            _productionShown = !_productionShown;
            foldoutAnimation.Play(_productionShown ? foldoutShowAnim : foldoutHideAnim);
        }

        /// <summary>
        /// Выход в главное меню
        /// </summary>
        private static void OnMainMenuClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            GameManager.Instance.SaveApplicationData();
            ScenesController.Instance.MessageBroker
                .Publish(new SceneLoadMessage()
                {
                    sceneType = SceneTypes.MainMenu
                });
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