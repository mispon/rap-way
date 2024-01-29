using System;
using System.Collections.Generic;
using Game.Scenes;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace UI.Base
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
                .Receive<SceneLoadMessage>()
                .Subscribe(msg => Dispose())
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            
            foreach (var container in uiElementContainers)
                container.Dispose();
        }
    }
}