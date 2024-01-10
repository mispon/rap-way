using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Game.UI
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private List<UIElementContainer> _uiElementContainers;
        
        protected override void InitializeSingleton()
        {
            base.InitializeSingleton();

            foreach (var container in _uiElementContainers)
                container.Initialize();
        }
    }
}