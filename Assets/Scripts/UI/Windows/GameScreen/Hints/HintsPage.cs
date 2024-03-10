using Core.Context;
using Enums;
using Firebase.Analytics;
using MessageBroker;
using MessageBroker.Messages.UI;
using UI.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Hints
{
    public class HintsPage : Page
    {
        [SerializeField] private Text info;
        [SerializeField] private Button okBtn;

        private object _pageCtx;
        
        private void Start()
        {
            okBtn.onClick.AddListener(() =>
            {
                MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.Previous, _pageCtx));
            });
        }

        public override void Show(object ctx = null)
        {
            _pageCtx  = ctx.ValueByKey<object>("page_ctx");
            
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