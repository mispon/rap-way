using Core;
using Game.UI.Messages;
using ScriptableObjects;
using UI.Enums;
using UI.MessageBroker;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

<<<<<<<< HEAD:Assets/Scripts/UI/Controls/Buttons/ShowOverlayWindowButton.cs
namespace UI.Controls.Buttons
========
namespace Game.UI.TutorialWindows
>>>>>>>> main:Assets/Scripts/UI/Controls/TutorialWindows/ShowTutorialWindowButton.cs
{
    [RequireComponent(typeof(Button))]
    public sealed class ShowTutorialWindowButton : MonoBehaviour
    {
        [SerializeField] private WindowType _toTutorialWindow;
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
