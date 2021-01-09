using Core;
using Data;
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
        [Header("Настройки UI")]
        [SerializeField] private Text descriptionText;
        [SerializeField] private Image backgroundImage;

        [Header("UI изменения метрик")] 
        [SerializeField] private Text moneyText;
        [SerializeField] private Text fansText;
        [SerializeField] private Text hypeText;
        [SerializeField] private Text expText;
        
        private GameEventDecision _eventDecision;
        
        /// <summary>
        /// Функция показа страницы решения по игровому событию
        /// </summary>
        public void Show(GameEventDecision eventDecision)
        {
            _eventDecision = eventDecision;
        }

        /// <summary>
        /// Применение изменений метрик
        /// </summary>
        private void SaveResult()
        {
            var income = _eventDecision.MetricsIncome;
            PlayerManager.Instance.GiveReward(income.Fans, income.Money, income.Experience);
            PlayerManager.Instance.AddHype(income.Hype);
        }

        protected override void BeforePageOpen()
        {
            descriptionText.text = _eventDecision.DecisionUi.Description;
            backgroundImage.sprite = _eventDecision.DecisionUi.Background;

            moneyText.SetMetricsInfo(_eventDecision.MetricsIncome.Money);
            fansText.SetMetricsInfo(_eventDecision.MetricsIncome.Fans);
            hypeText.SetMetricsInfo(_eventDecision.MetricsIncome.Hype);
            expText.SetMetricsInfo(_eventDecision.MetricsIncome.Experience);
        }

        protected override void BeforePageClose()
        {
            GameEventsManager.Instance.onEventShow?.Invoke();
        }

        protected override void AfterPageClose()
        {
            moneyText.ClearMetricsInfo();
            fansText.ClearMetricsInfo();
            hypeText.ClearMetricsInfo();
            expText.ClearMetricsInfo();

            descriptionText.text = "";
            backgroundImage.sprite = null;

            SaveResult();
            _eventDecision = null;
        }
    }

    public static class Extensions
    {
        /// <summary>
        /// Отображение объекта, содержащего инфомацию об изменении метрики, и наполнение его этой информацией
        /// </summary>
        public static void SetMetricsInfo(this Text component, int value)
        {
            if (value == 0)
            {
                component.text = $"{value.GetDisplay()}";
                component.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Сброс данных изменения метрики
        /// </summary>
        public static void ClearMetricsInfo(this Text component)
        {
            component.text = string.Empty;
            component.gameObject.SetActive(false);
        }
    }
}