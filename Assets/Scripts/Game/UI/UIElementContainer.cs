using Game.UI.Interfaces;
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
        protected CompositeDisposable disposables = new CompositeDisposable();

        /// <summary>
        /// Do not use the Awake method it is can be invoked the player return to main menu. 
        /// Use the Initialize method instead. 
        /// </summary>
        public virtual void Initialize()
        {
            Canvas = GetComponent<Canvas>();
            Canvas.enabled = false;

            uiMessageBroker = UIManager.Instance.MessageBroker;
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


        protected abstract void SetupListeners();
        protected virtual void DisposeListeners()
        {
            disposables?.Dispose();
        }
        
        public virtual void Dispose() { }
    }
}
