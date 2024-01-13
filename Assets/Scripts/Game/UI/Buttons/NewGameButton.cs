using Core;
using Data;
using Firebase.Analytics;
using Game.UI.Enums;
using Game.UI.Messages;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Buttons
{
    public class NewGameButton : MonoBehaviour
    {
        [SerializeField] private OverlayWindowType _toOverlayWindow;
        [SerializeField] private UIActionType _soundType = UIActionType.Click;

        private void Awake()
        {   
            var button = GetComponent<Button>();
            button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    FirebaseAnalytics.LogEvent(FirebaseGameEvents.NewGamePage);
                    
                    SoundManager.Instance.PlaySound(_soundType);
                    
                    UIMessageBroker.Instance.MessageBroker
                        .Publish(new OverlayWindowControlMessage()
                        {
                            Type = _toOverlayWindow
                        });
                });
        }
    }
}