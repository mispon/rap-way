using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Context;
using Enums;
using MessageBroker;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UI.Enums;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.Windows.GameScreen.GameEvent
{
    public class EventMainPage: Page
    {
        [BoxGroup("Card"), SerializeField] private Text nameText;
        [BoxGroup("Card"), SerializeField] private Text descriptionText;
        
        [BoxGroup("Decisions"), SerializeField] private Button peacefullyButton;
        [BoxGroup("Decisions"), SerializeField] private Button aggressivelyButton;
        [BoxGroup("Decisions"), SerializeField] private Button neutralButton;
        [BoxGroup("Decisions"), SerializeField] private Button randomButton;
        
        private GameEventInfo _eventInfo;

        private void Start()
        {
            peacefullyButton.onClick.AddListener(() => Decide(GameEventDecisionType.Peacefully));
            aggressivelyButton.onClick.AddListener(() => Decide(GameEventDecisionType.Aggressively));
            neutralButton.onClick.AddListener(() => Decide(GameEventDecisionType.Neutral));
            randomButton.onClick.AddListener(DecideRandom);
        }
        
        public override void Show(object ctx = null)
        {
            _eventInfo = ctx.Value<GameEventInfo>();
            base.Show(ctx);
        }
   
        private void Decide(GameEventDecisionType type)
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            
            var decisionResult = _eventInfo.DecisionResults.FirstOrDefault(e => e.DecisionType == type);
            if (decisionResult != null)
                MsgBroker.Instance.Publish(new WindowControlMessage
                {
                    Type = WindowType.GameEvent,
                    Context = new Dictionary<string, object>
                    {
                        ["event_name"]     = _eventInfo.Name,
                        ["event_decision"] = decisionResult
                    }
                });
            else
                Debug.LogError($"Не найден результат случайного события для решения типа {type}!");
        }

        private void DecideRandom()
        {
            var decisionTypes = (GameEventDecisionType[]) Enum.GetValues(typeof(GameEventDecisionType));
            Decide(decisionTypes[Random.Range(0, decisionTypes.Length)]);
        }
        
        protected override void BeforeShow(object ctx = null)
        {
            nameText.text = GetLocale(_eventInfo.Name);
            descriptionText.text = GetLocale(_eventInfo.Description);
        }

        protected override void AfterHide()
        {
            nameText.text = string.Empty;
            descriptionText.text = string.Empty;
            _eventInfo = null;
        }
    }
}