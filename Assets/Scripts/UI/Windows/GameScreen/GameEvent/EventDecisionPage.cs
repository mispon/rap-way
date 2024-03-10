using Core.Context;
using Extensions;
using Game;
using Game.Player.State.Desc;
using MessageBroker;
using MessageBroker.Messages.Production;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.GameEvent
{
    public class EventDecisionPage: Page
    {
        [BoxGroup("Card"), SerializeField] private Text nameText;
        [BoxGroup("Card"), SerializeField] private Text descriptionText;
        
        [BoxGroup("Incomes"), SerializeField] private Text moneyText;
        [BoxGroup("Incomes"), SerializeField] private Text fansText;
        [BoxGroup("Incomes"), SerializeField] private Text hypeText;
        [BoxGroup("Incomes"), SerializeField] private Text expText;
        
        [BoxGroup("Buttons"), SerializeField] private Button continueBtn;

        private GameEventDecision _eventDecision;

        private void Start()
        {
            continueBtn.onClick.AddListener(() => base.Hide());
        }

        public override void Show(object ctx = null)
        {
            var eventName = ctx.ValueByKey<string>("event_name");
            var eventDecision = ctx.ValueByKey<GameEventDecision>("event_decision");
            
            nameText.text = GetLocale(eventName);
            _eventDecision = eventDecision;
           
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