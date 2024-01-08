using Game.UI.Enums;
using Game.UI.Messages;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.OverlayWindows
{
    [RequireComponent(typeof(Button))]
    public sealed class ShowOverlayWindow : MonoBehaviour
    {
        [SerializeField]
        private OverlayWindowType _toOverlayWindow;

        private void Awake()
        {   
            var button = GetComponent<Button>();
            button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    UIManager.Instance.MessageBroker
                        .Publish(new OverlayWindowControlMessage()
                        {
                            OverlayWindowType = _toOverlayWindow
                        });
                });
        }
    }
}
