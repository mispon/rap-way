using System;
using System.Collections.Generic;
using Game.UI.Enums;
using Game.UI.Messages;
using Scenes;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace Game.UI
{
    public class UIManager : SerializedMonoBehaviour, IDisposable
    {
        [SerializeField] private List<UIElementContainer> uiElementContainers;
        
        private readonly CompositeDisposable _disposables = new();
        
        private void Start()
        {
            foreach (var container in uiElementContainers)
                container.Initialize();
            
            ScenesController.Instance.MessageBroker
                .Receive<SceneReadyMessage>()
                .Subscribe(msg =>
                {
                    if (msg.Type == SceneTypes.MainMenu)
                    {
                        MenuSceneInitialize();
                    }
                })
                .AddTo(_disposables);
            
            ScenesController.Instance.MessageBroker
                .Receive<SceneLoadMessage>()
                .Subscribe(msg => Dispose())
                .AddTo(_disposables);
        }

        private static void MenuSceneInitialize()
        {
            var uiBroker =  UIMessageBroker.Instance.MessageBroker;
            
            uiBroker.Publish(new WindowControlMessage {Type = WindowType.MainMenu});
            uiBroker.Publish(new OverlayWindowControlMessage {Type = OverlayWindowType.None});
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            
            foreach (var container in uiElementContainers)
                container.Dispose();
        }
    }
}