using Core;
using ScriptableObjects;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scenes
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
                            SceneType = _sceneTypes
                        });
                })
                .AddTo(this);
        }
    }
}
