using Core;
using Data;
using Game.UI.Enums;
using Game.UI.Messages;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.OverlayWindows
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
                    
                    UIManager.Instance.MessageBroker
                        .Publish(new OverlayWindowControlMessage()
                        {
                            OverlayWindowType = _toOverlayWindow
                        });
                });
        }
    }
}
