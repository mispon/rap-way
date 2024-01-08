using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Utils;

namespace Game.UI
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private List<UIElementContainer> _uiElementContainers;

        private MessageBroker _messageBroker;
        
        public MessageBroker MessageBroker => _messageBroker ??= new MessageBroker();
        
        protected override void InitializeSingleton()
        {
            base.InitializeSingleton();

            foreach (var container in _uiElementContainers)
                container.Initialize();
        }
    }
}