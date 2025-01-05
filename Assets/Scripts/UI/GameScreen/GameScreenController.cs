using System;
using System.Collections.Generic;
using Core;
using Core.OrderedStarter;
using Extensions;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.Player.State;
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
    public class GameScreenController : Singleton<GameScreenController>, IStarter
    {
        [SerializeField] private Image           playerAvatar;
        [SerializeField] private TextMeshProUGUI playerNickname;
        [SerializeField] private TextMeshProUGUI playerRealname;

        [SerializeField] private TextMeshProUGUI playerMoney;
        [SerializeField] private TextMeshProUGUI playerFans;
        [SerializeField] private TextMeshProUGUI playerHype;

        [SerializeField] private Text   currentDate;
        [SerializeField] private Button moneyButton;
        [SerializeField] private Button fansButton;
        [SerializeField] private Button hypeButton;

        [SerializeField] private StatDescItem[] statDescItems;

        private readonly CompositeDisposable _disposable = new();

        public void OnStart()
        {
            moneyButton.onClick.AddListener(() => ShowDescriptionPage(statDescItems[0]));
            fansButton.onClick.AddListener(() => ShowDescriptionPage(statDescItems[1]));
            hypeButton.onClick.AddListener(() => ShowDescriptionPage(statDescItems[2]));

            HandleStateEvents();

            MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.GameScreen));
        }

        private void HandleStateEvents()
        {
            MsgBroker.Instance
                .Receive<DayLeftMessage>()
                .Subscribe(e => OnDayLeft())
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<MoneyChangedMessage>()
                .Subscribe(e => playerMoney.text = e.NewVal.GetShort())
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<FansChangedMessage>()
                .Subscribe(e => playerFans.text = e.NewVal.GetShort())
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
            playerRealname.text = $"{resp.RealName}, {resp.Level} lvl";
            playerAvatar.sprite = SpritesManager.Instance.GetPortrait(resp.NickName);
            playerMoney.text    = resp.Money.GetShort();
            playerFans.text     = resp.Fans.GetShort();
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