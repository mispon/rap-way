using Sirenix.OdinInspector;
using UI.Base.Interfaces;
using UniRx;

namespace UI.Base
{
    public abstract class UIElementContainer : SerializedMonoBehaviour, IUIElementContainer
    {
        protected readonly CompositeDisposable disposables = new();
        
        public virtual void Initialize() {}

        public virtual void Dispose()
        {
            disposables?.Dispose();
        }
    }
}
