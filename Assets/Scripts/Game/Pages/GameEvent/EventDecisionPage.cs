using Data;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.GameEvent
{
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
        
        public void Show(GameEventDecision eventDecision)
        {
            _eventDecision = eventDecision;
        }

        private void SaveResult()
        {
            var income = _eventDecision.MetricsIncome;
            PlayerManager.Instance.AddMoney(income.Money);
            PlayerManager.Instance.AddFans(income.Fans);
            PlayerManager.Instance.AddHype(income.Hype);
            PlayerManager.Instance.AddExp(income.Experience);
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
        public static void SetMetricsInfo(this Text component, int value)
        {
            var isActive = (value == 0);
            component.gameObject.SetActive(isActive);
            if (isActive)
                component.text = value.GetDescription();
        }

        public static void ClearMetricsInfo(this Text component)
        {
            component.text = "";
            component.gameObject.SetActive(false);
        }
    }
}