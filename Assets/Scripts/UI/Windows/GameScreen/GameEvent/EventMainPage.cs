using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Context;
using Enums;
using MessageBroker;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using UI.Enums;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.Windows.GameScreen.GameEvent
{
    public class EventMainPage : Page
    {
        [Header("Card")]
        [SerializeField] private Text nameText;
        [SerializeField] private Text descriptionText;

        [Header("Decisions")]
        [SerializeField] private Button peacefullyButton;
        [SerializeField] private Button aggressivelyButton;
        [SerializeField] private Button neutralButton;
        [SerializeField] private Button randomButton;

        private GameEventInfo _eventInfo;
        private Action _closeCallback;

        private void Start()
        {
            peacefullyButton.onClick.AddListener(() => Decide(GameEventDecisionType.Peacefully));
            aggressivelyButton.onClick.AddListener(() => Decide(GameEventDecisionType.Aggressively));
            neutralButton.onClick.AddListener(() => Decide(GameEventDecisionType.Neutral));
            randomButton.onClick.AddListener(DecideRandom);
        }

        private void Decide(GameEventDecisionType type)
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);

            var decisionResult = _eventInfo.DecisionResults.FirstOrDefault(e => e.DecisionType == type);
            if (decisionResult != null)
            {
                MsgBroker.Instance.Publish(new WindowControlMessage
                {
                    Type = WindowType.GameEventResult,
                    Context = new Dictionary<string, object>
                    {
                        ["event_name"] = _eventInfo.Name,
                        ["event_decision"] = decisionResult,
                        ["close_callback"] = _closeCallback
                    }
                });
            }
            else
            {
                Debug.LogError($"Не найден результат случайного события для решения типа {type}!");
            }
        }

        private void DecideRandom()
        {
            var decisionTypes = (GameEventDecisionType[])Enum.GetValues(typeof(GameEventDecisionType));
            Decide(decisionTypes[Random.Range(0, decisionTypes.Length)]);
        }

        protected override void BeforeShow(object ctx = null)
        {
            _eventInfo = ctx.ValueByKey<GameEventInfo>("event_info");
            _closeCallback = ctx.ValueByKey<Action>("close_callback");

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