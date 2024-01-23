using Core;
using Data;
using Game.UI.Enums;
using Game.UI.Messages;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.TutorialWindows
{
    [RequireComponent(typeof(Button))]
    public sealed class ShowTutorialWindowButton : MonoBehaviour
    {
        [SerializeField] private TutorialWindowType _toTutorialWindow;
        [SerializeField] private UIActionType _soundType = UIActionType.Click;

        private void Awake()
        {   
            var button = GetComponent<Button>();
            button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    SoundManager.Instance.PlaySound(_soundType);
                    
                    UIMessageBroker.Instance.MessageBroker
                        .Publish(new TutorialWindowControlMessage()
                        {
                            Type = _toTutorialWindow
                        });
                });
        }
    }
}
