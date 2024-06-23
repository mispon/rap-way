using System;
using System.Collections.Generic;
using Core;
using Core.OrderedStarter;
using Enums;
using Extensions;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.Player.State;
using MessageBroker.Messages.SocialNetworks;
using MessageBroker.Messages.Time;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using TMPro;
using UI.Enums;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameScreen
{
    /// <summary>
    ///     Main game screen controller
    /// </summary>
    public class GameScreenController : Singleton<GameScreenController>, IStarter
    {
        [SerializeField] private Image           playerAvatar;
        [SerializeField] private TextMeshProUGUI playerNickname;
        [SerializeField] private TextMeshProUGUI playerLevel;
        [SerializeField] private Text            playerFans;
        [SerializeField] private Text            playerMoney;
        [SerializeField] private Text            playerHype;
        [SerializeField] private Text            currentDate;
        [SerializeField] private Button          moneyButton;
        [SerializeField] private Button          fansButton;
        [SerializeField] private Button          hypeButton;
        [SerializeField] private StatDescItem[]  statDescItems;

        [Space] [SerializeField] private ImagesBank imagesBank;
        [SerializeField]         private Button     testButton;

        private readonly CompositeDisposable _disposable = new();

        private void OnDestroy()
        {
            _disposable.Clear();
        }

        public void OnStart()
        {
            moneyButton.onClick.AddListener(() => ShowDescriptionPage(statDescItems[0]));
            fansButton.onClick.AddListener(() => ShowDescriptionPage(statDescItems[1]));
            hypeButton.onClick.AddListener(() => ShowDescriptionPage(statDescItems[2]));

            HandleStateEvents();

            MsgBroker.Instance.Publish(new FullStateRequest());
            MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.GameScreen));

            testButton.onClick.AddListener(() =>
            {
                MsgBroker.Instance.Publish(new EmailMessage
                {
                    Title       = "CASINO 0NLINE!",
                    Content     = "PLAY CASINO PLEASE PLAY CASINO PLAY OUR AWESOME CASINO",
                    Sender      = "stupid-casino777@gmail.com",
                    mainAction  = () => Debug.Log("main action"),
                    quickAction = () => Debug.Log("quick action")
                });
            });
        }

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
        }

        private static void ShowDescriptionPage(StatDescItem item)
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type = WindowType.StatsDesc,
                Context = new Dictionary<string, object>
                {
                    ["icon"]    = item.Icon,
                    ["nameKey"] = item.NameKey,
                    ["descKey"] = item.DescKey
                }
            });
        }

        private void UpdateHUD(FullStateResponse resp)
        {
            playerNickname.text = resp.NickName.ToUpper();
            playerAvatar.sprite = resp.Gender == Gender.Male ? imagesBank.MaleAvatar : imagesBank.FemaleAvatar;
            playerMoney.text    = resp.Money.GetMoney();
            playerFans.text     = resp.Fans.GetDisplay();
            playerHype.text     = resp.Hype.ToString();
            currentDate.text    = TimeManager.Instance.DisplayNow;
        }

        public void SetVisibility(bool state)
        {
            gameObject.SetActive(state);
        }

        private void OnDayLeft()
        {
            currentDate.text = TimeManager.Instance.DisplayNow;
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