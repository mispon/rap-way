using Game.UI.Enums;
using Game.UI.Messages;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Windows
{
    [RequireComponent(typeof(Button))]
    public sealed class ChangeWindowButton: MonoBehaviour
    {
        [SerializeField]
        private WindowType _toWindow;

        private void Awake()
        { 
            var button = GetComponent<Button>();
            button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    UIManager.Instance.MessageBroker
                        .Publish(new WindowControlMessage()
                        {
                            WindowType = _toWindow
                        });
                });
        }
    }
}
