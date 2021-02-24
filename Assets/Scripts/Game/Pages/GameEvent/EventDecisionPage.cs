using Core;
using Data;
using Models.Player;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.GameEvent
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
            nameText.text = eventName;
            _eventDecision = eventDecision;
            Open();
        }

        /// <summary>
        /// Применение изменений метрик
        /// </summary>
        private void SaveResult()
        {
            var income = CalculateIncome(PlayerManager.Data, _eventDecision);
            
            PlayerManager.Instance.GiveReward(income.Fans, income.Money, income.Exp);
            PlayerManager.Instance.AddHype(income.Hype);
        }

        /// <summary>
        /// Считает доход игрока
        /// </summary>
        private static MetricsIncome CalculateIncome(PlayerData playerData, GameEventDecision decision)
        {
            return new MetricsIncome
            {
                // NOTE: Изменение денег тоже зависит от фанатов
                Money = Mathf.RoundToInt(playerData.Fans * decision.MoneyChange),
                Fans = Mathf.RoundToInt(playerData.Fans * decision.FansChange),
                Hype = decision.HypeChange,
                Exp = decision.ExpChange
            };
        }

        protected override void BeforePageOpen()
        {
            descriptionText.text = _eventDecision.Description;

            var income = CalculateIncome(PlayerManager.Data, _eventDecision);
            
            moneyText.text = income.Money.GetMoney();
            fansText.text = income.Fans.GetDisplay();
            hypeText.text = income.Hype.ToString();
            expText.text = income.Exp.ToString();
        }
        
        protected override void BeforePageClose()
        {
            GameEventsManager.Instance.onEventShow?.Invoke();
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