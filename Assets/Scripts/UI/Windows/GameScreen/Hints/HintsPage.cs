using System;
using Core.Context;
using Enums;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Hints
{
    public class HintsPage : Page
    {
        [SerializeField] private Text info;
        [SerializeField] private Button okBtn;

        private void Start()
        {
            okBtn.onClick.AddListener(() => base.Hide());
        }

        public override void Show(object ctx = null)
        {
            var key      = ctx.ValueByKey<string>("hint_key");
            var infoText = ctx.ValueByKey<string>("hint_text");
            
            if (key == "tutorial_on_start")
            {
                FirebaseAnalytics.LogEvent(FirebaseGameEvents.FirstHintOK);
            }
            
            info.text = infoText;
            base.Show(ctx);
        }
    }
}