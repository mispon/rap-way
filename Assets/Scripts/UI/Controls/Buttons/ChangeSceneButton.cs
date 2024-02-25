using Core;
using Scenes.MessageBroker;
using Scenes.MessageBroker.Messages;
using ScriptableObjects;
using UI.Enums;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Controls.Buttons
{
    [RequireComponent(typeof(Button))]
    public sealed class ChangeSceneButton: MonoBehaviour
    {
        [FormerlySerializedAs("_sceneTypes")] [SerializeField] private SceneType sceneType;
        [SerializeField] private UIActionType _soundType = UIActionType.Click;

        private void Awake()
        {
            var button = GetComponent<Button>();
            button
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    SoundManager.Instance.PlaySound(_soundType);
                    SceneMessageBroker.Instance.Publish(new SceneLoadMessage {SceneType = sceneType});
                })
                .AddTo(this);
        }
    }
}
