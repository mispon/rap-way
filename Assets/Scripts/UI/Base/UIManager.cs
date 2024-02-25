using System;
using System.Collections.Generic;
using Scenes.MessageBroker;
using Scenes.MessageBroker.Messages;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace UI.Base
{
    public class UIManager : SerializedMonoBehaviour, IDisposable
    {
        [SerializeField] private UIElementContainer[] containers;

        private readonly CompositeDisposable _disposables = new();

        private void Start()
        {
            foreach (var container in containers)
                container.Initialize();
            
            SceneMessageBroker.Instance
                .Receive<SceneLoadMessage>()
                .Subscribe(msg => Dispose())
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            
            foreach (var container in containers)
                container.Dispose();
        }
    }
}