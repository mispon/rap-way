﻿using System;
using Core;
using Core.OrderedStarter;
using Enums;
using Extensions;
using Game;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.Player.State;
using MessageBroker.Messages.Time;
using Scenes.MessageBroker;
using Scenes.MessageBroker.Messages;
using ScriptableObjects;
using UI.Enums;
using UI.MessageBroker;
using UI.MessageBroker.Messages;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameScreen
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

        private bool _productionShown;
        
        public void OnStart()
        {
            moneyButton.onClick.AddListener(() => ShowDescriptionPage(statDescItems[0]));
            fansButton.onClick.AddListener(() => ShowDescriptionPage(statDescItems[1]));
            hypeButton.onClick.AddListener(() => ShowDescriptionPage(statDescItems[2]));

            productionFoldoutButton.onClick.AddListener(OnProductionClick);
            mainMenuButton.onClick.AddListener(OnMainMenuClick);
            
            HandleStateEvents();
        }

        /// <summary>
        /// Handles player's state updates
        /// </summary>
        private void HandleStateEvents()
        {
            MainMessageBroker.Instance
                .Receive<DayLeftMessage>()
                .Subscribe(e => OnDayLeft())
                .AddTo(_disposable);
            MainMessageBroker.Instance
                .Receive<MoneyChangedMessage>()
                .Subscribe(e => playerMoney.text = e.NewVal.GetMoney())
                .AddTo(_disposable);
            MainMessageBroker.Instance
                .Receive<FansChangedMessage>()
                .Subscribe(e => playerFans.text = e.NewVal.GetDisplay())
                .AddTo(_disposable);
            MainMessageBroker.Instance
                .Receive<HypeChangedMessage>()
                .Subscribe(e => playerHype.text = e.NewVal.ToString())
                .AddTo(_disposable);
            MainMessageBroker.Instance
                .Receive<FullStateResponse>()
                .Subscribe(UpdateHUD)
                .AddTo(_disposable);
            
            MainMessageBroker.Instance.Publish(new FullStateRequest());
            
            UIMessageBroker.Instance.Publish(new WindowControlMessage
            {
                Type = WindowType.GameScreen
            });
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
            SceneMessageBroker.Instance.Publish(new SceneLoadMessage {SceneType = SceneType.MainMenu});
        }
        
        private void OnDestroy()
        {
            _disposable.Clear();
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