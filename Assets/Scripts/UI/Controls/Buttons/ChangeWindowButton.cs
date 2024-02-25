using Core;
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
    public sealed class ChangeWindowButton: MonoBehaviour
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
                    MsgBroker.Instance.Publish(new WindowControlMessage(_toWindow));
                });
        }
    }
}
