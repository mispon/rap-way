using Core;
using Game.UI.Messages;
using ScriptableObjects;
using UI.Enums;
using UI.MessageBroker;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Controls.Buttons
{
    [RequireComponent(typeof(Button))]
    public sealed class ShowOverlayWindowButton : MonoBehaviour
    {
        [SerializeField] private OverlayWindowType _toOverlayWindow;
        [SerializeField] private UIActionType _soundType = UIActionType.Click;

        private void Awake()
        {   
            var button = GetComponent<Button>();
            button.OnClickAsObservable()
                .Subscribe(_ =>
                {
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
