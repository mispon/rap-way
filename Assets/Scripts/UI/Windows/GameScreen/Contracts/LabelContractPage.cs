using Core;
using Core.Context;
using Enums;
// using Firebase.Analytics;
using Game.Labels.Desc;
using MessageBroker;
using MessageBroker.Messages.Labels;
using MessageBroker.Messages.SocialNetworks;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Contracts
{
    public class LabelContractPage : Page
    {
        [SerializeField] private Image logo;
        [SerializeField] private Text greeting;
        [SerializeField] private Text contract;

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
                return;

            var label = ctx.Value<LabelInfo>();

            string playerNickname = PlayerAPI.Data.Info.NickName;
            _labelName = label.Name;

            logo.sprite = label.Logo;
            greeting.text = GetLocale("label_contract_greeting", _labelName);
            contract.text = GetLocale("label_contract_text", playerNickname, _labelName, _labelName);

            base.Show(ctx);
        }

        private void OnReject()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            // FirebaseAnalytics.LogEvent(FirebaseGameEvents.LabelContractDeclined);

            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text = "news_player_reject_label",
                TextArgs = new[] {
                        PlayerAPI.Data.Info.NickName,
                        _labelName
                    },
                Sprite = PlayerAPI.Data.Info.Gender == Gender.Male
                    ? imagesBank.MaleAvatar
                    : imagesBank.FemaleAvatar,
                Popularity = PlayerAPI.Data.Fans
            });

            base.Hide();
        }

        private void OnSign()
        {
            MsgBroker.Instance.Publish(new PlayerSignLabelsContractMessage { });

            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text = "news_player_join_label",
                TextArgs = new[] {
                        PlayerAPI.Data.Info.NickName,
                        _labelName
                    },
                Sprite = PlayerAPI.Data.Info.Gender == Gender.Male
                    ? imagesBank.MaleAvatar
                    : imagesBank.FemaleAvatar,
                Popularity = PlayerAPI.Data.Fans
            });

            SoundManager.Instance.PlaySound(UIActionType.Click);
            // FirebaseAnalytics.LogEvent(FirebaseGameEvents.LabelContractAccepted);
            PlayerAPI.Data.Label = _labelName;
            base.Hide();
        }

        protected override void AfterHide()
        {
            _labelName = "";
        }
    }
}