using Core;
using Enums;
// using Firebase.Analytics;
using MessageBroker;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using UI.Enums;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Controls.Buttons
{
    [RequireComponent(typeof(Button))]
    public class NewGameButton : MonoBehaviour
    {
        [SerializeField] private WindowType _toWindow;
        [SerializeField] private UIActionType _soundType = UIActionType.Click;

        private void Awake()
        {   
            var button = GetComponent<Button>();
            button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    SoundManager.Instance.PlaySound(_soundType);
                    // FirebaseAnalytics.LogEvent(FirebaseGameEvents.NewGamePage);

                    MsgBroker.Instance.Publish(new WindowControlMessage(_toWindow));
                });
        }
    }
}