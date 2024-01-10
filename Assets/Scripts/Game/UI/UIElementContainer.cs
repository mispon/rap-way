using Game.UI.Interfaces;
using Game.UI.Messages;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace Game.UI
{
    [RequireComponent(typeof(Canvas))]
    public abstract class UIElementContainer : SerializedMonoBehaviour, IUIElementContainer
    {
        public Canvas Canvas { get; private set; }
        public bool IsActive { get; private set; } = false;

        protected MessageBroker uiMessageBroker;
        protected readonly CompositeDisposable disposables = new CompositeDisposable();
        
        public virtual void Initialize()
        {
            Canvas = GetComponent<Canvas>();
            Canvas.enabled = false;

            uiMessageBroker = UIMessageBroker.Instance.MessageBroker;
        }

        protected virtual void Activate()
        {
            if (IsActive == true) return;
            IsActive = true;
            Canvas.enabled = true;
            
        }

        protected virtual void Deactivate()
        {
            if (IsActive == false) return;
            IsActive = false;
            Canvas.enabled = false;
            Dispose();
        }

        protected virtual void SetupListeners() { }

        protected virtual void DisposeListeners()
        {
            disposables?.Dispose();
        }
        
        public virtual void Dispose() { }
    }
}
