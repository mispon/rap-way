using System.Collections.Generic;
using Game.UI.Enums;
using Game.UI.Messages;
using Scenes;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace Game.UI
{
    public class UIManager : SerializedMonoBehaviour
    {
        [SerializeField] private List<UIElementContainer> _uiElementContainers;
        
        private void Awake()
        {
            foreach (var container in _uiElementContainers)
                container.Initialize();
            
            ScenesController.Instance.MessageBroker
                .Receive<SceneReadyMessage>()
                .Subscribe(msg =>
                {
                    if (msg.sceneType == SceneTypes.MainMenu) MenuSceneInitialize();
                    if (msg.sceneType == SceneTypes.Game) GameSceneInitialize();
                })
                .AddTo(this);
        }

        private void MenuSceneInitialize()
        {
            UIMessageBroker.Instance.MessageBroker
                .Publish(new WindowControlMessage()
                {
                    Type = WindowType.MainMenu
                });
            
            UIMessageBroker.Instance.MessageBroker
                .Publish(new OverlayWindowControlMessage()
                {
                    Type = OverlayWindowType.None
                });
        }
        
        private void GameSceneInitialize()
        {
            
        }
    }
}