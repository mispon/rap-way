using System;
using Core.Context;
using Extensions;
using Game;
using Game.Player.State.Desc;
using MessageBroker;
using MessageBroker.Messages.Production;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.GameEvent
{
    public class EventDecisionPage : Page
    {
        [Header("Card")]
        [SerializeField] private Text nameText;
        [SerializeField] private Text descriptionText;

        [Header("Incomes")]
        [SerializeField] private Text moneyText;
        [SerializeField] private Text fansText;
        [SerializeField] private Text hypeText;
        [SerializeField] private Text expText;

        [SerializeField] private Button continueBtn;

        private GameEventDecision _eventDecision;
        private Action _closeCallback;

        private void Start()
        {
            continueBtn.onClick.AddListener(() =>
            {
                _closeCallback.Invoke();
            });
        }

        public override void Show(object ctx = null)
        {
            var eventName = ctx.ValueByKey<string>("event_name");
            _eventDecision = ctx.ValueByKey<GameEventDecision>("event_decision");
            _closeCallback = ctx.ValueByKey<Action>("close_callback");

            nameText.text = GetLocale(eventName);

            base.Show(ctx);
        }

        private void SaveResult()
        {
            var income = CalculateIncome(PlayerAPI.Data, _eventDecision);
            MsgBroker.Instance.Publish(new ProductionRewardMessage
            {
                MoneyIncome = income.Money,
                FansIncome = income.Fans,
                HypeIncome = income.Hype,
                Exp = income.Exp,
            });
        }

        private static MetricsIncome CalculateIncome(PlayerData playerData, GameEventDecision decision)
        {
            var settings = GameManager.Instance.Settings;
            int fansAmount = Mathf.Max(playerData.Fans, settings.Player.BaseFans);

            return new MetricsIncome
            {
                // NOTE: Изменение денег тоже зависит от фанатов
                Money = Mathf.RoundToInt(fansAmount * decision.MoneyChange),
                Fans = Mathf.RoundToInt(fansAmount * decision.FansChange),
                Hype = decision.HypeChange,
                Exp = decision.ExpChange
            };
        }

        protected override void BeforeShow(object ctx = null)
        {
            descriptionText.text = GetLocale(_eventDecision.Description);

            var income = CalculateIncome(PlayerAPI.Data, _eventDecision);

            moneyText.text = income.Money.GetMoney();
            fansText.text = income.Fans.GetDisplay();
            hypeText.text = income.Hype.ToString();
            expText.text = income.Exp.ToString();
        }

        protected override void AfterHide()
        {
            moneyText.text = string.Empty;
            fansText.text = string.Empty;
            hypeText.text = string.Empty;
            expText.text = string.Empty;

            SaveResult();

            nameText.text = string.Empty;
            descriptionText.text = string.Empty;
            _eventDecision = null;
        }
    }

    public class MetricsIncome
    {
        public int Money;
        public int Fans;
        public int Hype;
        public int Exp;
    }
}