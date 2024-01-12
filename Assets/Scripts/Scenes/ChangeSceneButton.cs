using Core;
using Data;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes
{
    [RequireComponent(typeof(Button))]
    public sealed class ChangeSceneButton: MonoBehaviour
    {
        [SerializeField] private SceneTypes _sceneTypes;
        [SerializeField] private UIActionType _soundType = UIActionType.Click;

        private void Awake()
        {
            var button = GetComponent<Button>();
            button
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    SoundManager.Instance.PlaySound(_soundType);
                    
                    ScenesController.Instance.MessageBroker
                        .Publish(new SceneLoadMessage()
                        {
                            Type = _sceneTypes
                        });
                })
                .AddTo(this);
        }
    }
}
