using System;
using System.Linq;
using Data;
using Enums;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.Pages.GameEvent
{
    /// <summary>
    /// Страница игрового события, описывающая ситуацию
    /// </summary>
    public class EventMainPage: Page
    {
        [Header("Поля")]
        [SerializeField] private Text nameText;
        [SerializeField] private Text descriptionText;

        [Header("Кнопки выбора решения")] 
        [SerializeField] private Button peacefullyButton;
        [SerializeField] private Button aggressivelyButton;
        [SerializeField] private Button neutralButton;
        [SerializeField] private Button randomButton;

        [Header("Страница результат выбора")] 
        [SerializeField] private EventDecisionPage eventDecisionPage;
        
        private GameEventInfo _eventInfo;

        private void Start()
        {
            peacefullyButton.onClick.AddListener(() => Decide(GameEventDecisionType.Peacefully));
            aggressivelyButton.onClick.AddListener(() => Decide(GameEventDecisionType.Aggressively));
            neutralButton.onClick.AddListener(() => Decide(GameEventDecisionType.Neutral));
            randomButton.onClick.AddListener(DecideRandom);
        }

        /// <summary>
        /// Функция показа страницы игрового события
        /// </summary>
        public void Show(GameEventInfo eventInfo)
        {
            _eventInfo = eventInfo;
            Open();
        }
        
        /// <summary>
        /// Выбирает решение определенного типа и отображает его на странице "eventDecisionPage"
        /// </summary>
        private void Decide(GameEventDecisionType type)
        {
            var decisionResult = _eventInfo.DecisionResults.FirstOrDefault(e => e.DecisionType == type);
            if (decisionResult != null)
                eventDecisionPage.Show(_eventInfo.Name, decisionResult);
            else
                Debug.LogError($"Не найден результат случайного события для решения типа {type}!");

            Close();
        }

        /// <summary>
        /// Выбирает случайное решение
        /// </summary>
        private void DecideRandom()
        {
            var decisionTypes = (GameEventDecisionType[]) Enum.GetValues(typeof(GameEventDecisionType));
            Decide(decisionTypes[Random.Range(0, decisionTypes.Length)]);
        }
        
        protected override void BeforePageOpen()
        {
            nameText.text = GetLocale(_eventInfo.Name);
            descriptionText.text = GetLocale(_eventInfo.Description);
        }

        protected override void AfterPageClose()
        {
            nameText.text = string.Empty;
            descriptionText.text = string.Empty;
            _eventInfo = null;
        }
    }
}