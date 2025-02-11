﻿using Core;
using Core.Analytics;
using Core.Context;
using Enums;
using Game.Labels.Desc;
using MessageBroker;
using MessageBroker.Messages.Labels;
using MessageBroker.Messages.SocialNetworks;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using UI.Enums;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Contracts
{
    public class LabelContractPage : Page
    {
        [SerializeField] private Image logo;
        [SerializeField] private Text  greeting;
        [SerializeField] private Text  contract;

        [Space]
        [SerializeField] private Button rejectButton;
        [SerializeField] private Button signButton;

        [Space]
        [SerializeField] private ImagesBank imagesBank;

        private string _labelName;

        private void Start()
        {
            rejectButton.onClick.AddListener(OnReject);
            signButton.onClick.AddListener(OnSign);
        }

        public override void Show(object ctx = null)
        {
            if (PlayerAPI.Data.Label != "")
            {
                return;
            }

            var label = ctx.Value<LabelInfo>();

            var playerNickname = PlayerAPI.Data.Info.NickName;
            _labelName = label.Name;

            logo.sprite   = label.Logo;
            greeting.text = GetLocale("label_contract_greeting", _labelName);
            contract.text = GetLocale("label_contract_text", playerNickname, _labelName, _labelName);

            base.Show(ctx);
        }

        private void OnReject()
        {
            AnalyticsManager.LogEvent(FirebaseGameEvents.LabelContractDeclined);
            SoundManager.Instance.PlaySound(UIActionType.Click);

            var nickname = PlayerAPI.Data.Info.NickName;

            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text       = "news_player_reject_label",
                TextArgs   = new[] {nickname, _labelName},
                Sprite     = SpritesManager.Instance.GetPortrait(nickname),
                Popularity = PlayerAPI.Data.Fans
            });

            MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.Previous));
        }

        private void OnSign()
        {
            AnalyticsManager.LogEvent(FirebaseGameEvents.LabelContractAccepted);
            SoundManager.Instance.PlaySound(UIActionType.Click);

            PlayerAPI.Data.Label = _labelName;
            var nickname = PlayerAPI.Data.Info.NickName;

            MsgBroker.Instance.Publish(new PlayerSignLabelsContractMessage());
            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text       = "news_player_join_label",
                TextArgs   = new[] {nickname, _labelName},
                Sprite     = SpritesManager.Instance.GetPortrait(nickname),
                Popularity = PlayerAPI.Data.Fans
            });

            MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.Previous));
        }

        protected override void AfterHide()
        {
            _labelName = "";
        }
    }
}