using Extensions;
using Game;
using Game.Player;
using MessageBroker;
using MessageBroker.Messages.Production;
using Models.Player;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Pages.GameEvent
{
    /// <summary>
    /// Страница, описывающая принятое решение по игровому событию
    /// </summary>
    public class EventDecisionPage: Page
    {
        [Header("Поля")]
        [SerializeField] private Text nameText;
        [SerializeField] private Text descriptionText;

        [Header("Индикаторы изменения параметров")] 
        [SerializeField] private Text moneyText;
        [SerializeField] private Text fansText;
        [SerializeField] private Text hypeText;
        [SerializeField] private Text expText;

        private GameEventDecision _eventDecision;
        
        /// <summary>
        /// Функция показа страницы решения по игровому событию
        /// </summary>
        public void Show(string eventName, GameEventDecision eventDecision)
        {
            nameText.text = GetLocale(eventName);
            _eventDecision = eventDecision;
            Open();
        }

        /// <summary>
        /// Применение изменений метрик
        /// </summary>
        private void SaveResult()
        {
            var income = CalculateIncome(PlayerManager.Data, _eventDecision);
            MainMessageBroker.Instance.Publish(new ProductionRewardEvent
            {
                MoneyIncome = income.Money,
                FansIncome = income.Fans,
                HypeIncome = income.Hype,
                Exp = income.Exp,
            });
        }

        /// <summary>
        /// Считает доход игрока
        /// </summary>
        private static MetricsIncome CalculateIncome(PlayerData playerData, GameEventDecision decision)
        {
            int fansAmount = Mathf.Max(playerData.Fans, GameManager.Instance.Settings.BaseFans);

            return new MetricsIncome
            {
                // NOTE: Изменение денег тоже зависит от фанатов
                Money = Mathf.RoundToInt(fansAmount * decision.MoneyChange),
                Fans = Mathf.RoundToInt(fansAmount * decision.FansChange),
                Hype = decision.HypeChange,
                Exp = decision.ExpChange
            };
        }

        protected override void BeforePageOpen()
        {
            descriptionText.text = GetLocale(_eventDecision.Description);

            var income = CalculateIncome(PlayerManager.Data, _eventDecision);
            
            moneyText.text = income.Money.GetMoney();
            fansText.text = income.Fans.GetDisplay();
            hypeText.text = income.Hype.ToString();
            expText.text = income.Exp.ToString();
        }

        protected override void AfterPageClose()
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