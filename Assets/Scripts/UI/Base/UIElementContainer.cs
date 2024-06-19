using UI.Base.Interfaces;
using UniRx;
using UnityEngine;

namespace UI.Base
{
    public abstract class UIElementContainer : MonoBehaviour, IUIElementContainer
    {
        protected readonly CompositeDisposable disposables = new();
        
        public virtual void Initialize() {}

        public virtual void Dispose()
        {
            disposables?.Dispose();
        }
    }
}
