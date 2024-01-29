using Sirenix.OdinInspector;
using UI.Base.Interfaces;
using UniRx;
using UnityEngine;

namespace UI.Base
{
    [RequireComponent(typeof(Canvas))]
    public abstract class UIElementContainer : SerializedMonoBehaviour, IUIElementContainer
    {
        public Canvas Canvas { get; private set; }
        public bool IsActive { get; private set; }
        
        protected readonly CompositeDisposable disposables = new();
        
        public virtual void Initialize()
        {
            Canvas = GetComponent<Canvas>();
            Canvas.enabled = false;
        }

        protected void Activate()
        {
            if (IsActive) 
                return;
            
            IsActive = true;
            Canvas.enabled = true;
        }

        protected virtual void Deactivate()
        {
            if (IsActive == false) return;
            IsActive = false;
            Canvas.enabled = false;
        }

        public virtual void Dispose()
        {
            DisposeListeners();
        }
        
        private void DisposeListeners()
        {
            disposables?.Dispose();
        }
    }
}
