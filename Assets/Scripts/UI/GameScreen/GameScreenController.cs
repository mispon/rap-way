using System;
using System.Collections.Generic;
using Core;
using Core.OrderedStarter;
using Enums;
using Extensions;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.Player.State;
using MessageBroker.Messages.Time;
using MessageBroker.Messages.UI;
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

        [BoxGroup("Other"), SerializeField] private ImagesBank imagesBank;
        
        private readonly CompositeDisposable _disposable = new();
        
        public void OnStart()
        {
            moneyButton.onClick.AddListener(() => ShowDescriptionPage(statDescItems[0]));
            fansButton.onClick.AddListener(() => ShowDescriptionPage(statDescItems[1]));
            hypeButton.onClick.AddListener(() => ShowDescriptionPage(statDescItems[2]));

            HandleStateEvents();
            
            MsgBroker.Instance.Publish(new FullStateRequest());
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
                    ["icon"] = item.Icon,
                    ["nameKey"] = item.NameKey,
                    ["descKey"] = item.DescKey,
                }
            });
        }

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