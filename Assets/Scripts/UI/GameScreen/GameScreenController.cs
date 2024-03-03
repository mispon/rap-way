using System;
using System.Collections.Generic;
using Core;
using Core.OrderedStarter;
using Enums;
using Extensions;
using Game;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.Player.State;
using MessageBroker.Messages.Time;
using MessageBroker.Messages.UI;
using Scenes.MessageBroker;
using Scenes.MessageBroker.Messages;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UI.Enums;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameScreen
{
    /// <summary>
    /// Main game screen controller
    /// </summary>
    public class GameScreenController: Singleton<GameScreenController>, IStarter
    {
        [BoxGroup("HUD"), SerializeField] private Image playerAvatar;
        [BoxGroup("HUD"), SerializeField] private Text playerNickname;
        [BoxGroup("HUD"), SerializeField] private Text playerFans;
        [BoxGroup("HUD"), SerializeField] private Text playerMoney;
        [BoxGroup("HUD"), SerializeField] private Text playerHype;
        [BoxGroup("HUD"), SerializeField] private Text currentDate;
        [BoxGroup("HUD"), SerializeField] private Button moneyButton;
        [BoxGroup("HUD"), SerializeField] private Button fansButton;
        [BoxGroup("HUD"), SerializeField] private Button hypeButton;
        [BoxGroup("HUD"), SerializeField] private StatDescItem[] statDescItems;
        
        [BoxGroup("Actions"), SerializeField] private Button productionFoldoutButton;
        [BoxGroup("Actions"), SerializeField] private Animation foldoutAnimation;
        [BoxGroup("Actions"), SerializeField] private string foldoutShowAnim;
        [BoxGroup("Actions"), SerializeField] private string foldoutHideAnim;

        [BoxGroup("Other"), SerializeField] private ImagesBank imagesBank;
        [BoxGroup("Other"), SerializeField] private Button mainMenuButton;
        
        private bool _productionShown;
        private readonly CompositeDisposable _disposable = new();
        
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
            MsgBroker.Instance
                .Receive<DayLeftMessage>()
                .Subscribe(e => OnDayLeft())
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<MoneyChangedMessage>()
                .Subscribe(e => playerMoney.text = e.NewVal.GetMoney())
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<FansChangedMessage>()
                .Subscribe(e => playerFans.text = e.NewVal.GetDisplay())
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<HypeChangedMessage>()
                .Subscribe(e => playerHype.text = e.NewVal.ToString())
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<FullStateResponse>()
                .Subscribe(UpdateHUD)
                .AddTo(_disposable);
            
            MsgBroker.Instance.Publish(new FullStateRequest());
            MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.GameScreen));
        }
        
        /// <summary>
        /// Открывает страницу с описанием основной характеристики
        /// </summary>
        private static void ShowDescriptionPage(StatDescItem item)
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type = WindowType.StatsDesc,
                Context = new Dictionary<string, object>
                {
                    ["icon"] = item.Icon,
                    ["nameKey"] = item.NameKey,
                    ["descKey"] = item.DescKey,
                }
            });
        }
        
        /// <summary>
        /// Updates main HUD
        /// </summary>
        private void UpdateHUD(FullStateResponse resp)
        {
            playerNickname.text = resp.NickName.ToUpper();
            playerAvatar.sprite = resp.Gender == Gender.Male ? imagesBank.MaleAvatar : imagesBank.FemaleAvatar;
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