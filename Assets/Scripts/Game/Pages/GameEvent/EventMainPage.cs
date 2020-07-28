using Data;
using Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.GameEvent
{
    public class EventMainPage: Page
    {
        [Header("Настройки UI")]
        [SerializeField] private Text descriptionText;
        [SerializeField] private Image backgroundImage;

        [Header("Кнопки выбора решения")] 
        [SerializeField] private Button peacefullyButton;
        [SerializeField] private Button aggressivelyButton;
        [SerializeField] private Button indifferentlyButton;
        [SerializeField] private Button peerAssistButton;

        [Header("Страница результат выбора")] 
        [SerializeField] private EventDecisionPage eventDecisionPage;
        
        private GameEventInfo _eventInfo;

        private void Start()
        {
            peacefullyButton.onClick.AddListener(()=> Decide(GameEventDecisionType.Peacefully));
            aggressivelyButton.onClick.AddListener(()=> Decide(GameEventDecisionType.Aggressively));
            indifferentlyButton.onClick.AddListener(()=> Decide(GameEventDecisionType.Indifferently));
            peerAssistButton.onClick.AddListener(()=> Decide(GameEventDecisionType.PeerAssist));
        }

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
            eventDecisionPage.Show(_eventInfo.GetRandomDecision(type));
            Close();
        }

        protected override void BeforePageOpen()
        {
            descriptionText.text = _eventInfo.SituationUi.Description;
            backgroundImage.sprite = _eventInfo.SituationUi.Background;    
        }

        protected override void AfterPageClose()
        {
            descriptionText.text = "";
            backgroundImage.sprite = null;

            _eventInfo = null;
        }
    }
}