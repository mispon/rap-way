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

        private UniRx.MessageBroker uiMessageBroker;
        private readonly CompositeDisposable _disposables = new();

        private void Awake()
        {
            uiMessageBroker = UIMessageBroker.Instance.MessageBroker;
        }

        private void Start()
        {
            foreach (var container in uiElementContainers)
                container.Initialize();
            
            ScenesController.Instance.MessageBroker
                .Receive<SceneReadyMessage>()
                .Subscribe(msg =>
                {
                    if (msg.SceneType == SceneTypes.MainMenu)
                    {
                        if (msg.SceneType == SceneTypes.MainMenu) MenuSceneInitialize();
                        if (msg.SceneType == SceneTypes.Game) GameSceneInitialize();
                    }
                })
                .AddTo(_disposables);
            
            ScenesController.Instance.MessageBroker
                .Receive<SceneLoadMessage>()
                .Subscribe(msg => Dispose())
                .AddTo(_disposables);
        }

        private void MenuSceneInitialize()
        {
            uiMessageBroker.Publish(new WindowControlMessage {Type = WindowType.MainMenu});
            uiMessageBroker.Publish(new OverlayWindowControlMessage {Type = OverlayWindowType.None});
            uiMessageBroker.Publish(new TutorialWindowControlMessage() {Type = TutorialWindowType.None});
        }
        
        private void GameSceneInitialize()
        {
            uiMessageBroker.Publish(new WindowControlMessage {Type = WindowType.None});
            uiMessageBroker.Publish(new OverlayWindowControlMessage {Type = OverlayWindowType.None});
            uiMessageBroker.Publish(new TutorialWindowControlMessage() {Type = TutorialWindowType.None});
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            
            foreach (var container in uiElementContainers)
                container.Dispose();
        }
    }
}