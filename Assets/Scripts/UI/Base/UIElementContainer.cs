using Sirenix.OdinInspector;
using UI.Base.Interfaces;
using UniRx;
using UnityEngine;

namespace UI.Base
{
    public abstract class UIElementContainer : SerializedMonoBehaviour, IUIElementContainer
    {
        public bool IsActive { get; private set; }
        
        protected readonly CompositeDisposable disposables = new();
        
        public virtual void Initialize() {}

        protected void Activate()
        {
            if (IsActive) 
                return;
            
            IsActive = true;
        }

        protected virtual void Deactivate()
        {
            if (IsActive == false) 
                return;
            
            IsActive = false;
        }

        public virtual void Dispose()
        {
            disposables?.Dispose();
        }
    }
}
