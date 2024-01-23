using System;
using Sirenix.OdinInspector;
using UI.Base.Interfaces;
using UI.MessageBroker;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Base
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup), typeof(GraphicRaycaster))]
    public abstract class CanvasUIElement : SerializedMonoBehaviour, IUIElement, IDisposable
    {
        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        
        public bool IsActive => _isActive;
        public Canvas Canvas => _canvas;
        public CanvasGroup CanvasGroup => _canvasGroup;

        private bool _isActive;
        protected UniRx.MessageBroker uiMessageBus;
        
        private CompositeDisposable _disposables;

        public virtual void Initialize()
        {
            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
            
            uiMessageBus = UIMessageBroker.Instance.MessageBroker;
            _disposables = new CompositeDisposable();

            Hide();
            SetupListenersOnInitialize();
        }
        
        public virtual void Show()
        {
            _isActive = true;
            _canvasGroup.interactable = true;
            _canvas.enabled = true;
            SetupListenersOnShow();
            SendRequests();
        }
        
        public virtual void Hide()
        {
            _isActive = false;
            _canvasGroup.interactable = false;
            _canvas.enabled = false;
            DisposeContainers();
            DisposeListeners();
        }
        
        protected virtual void SetupListenersOnInitialize() { }
        protected virtual void SetupListenersOnShow() { }
        protected virtual void SendRequests() { }
        protected virtual void DisposeContainers() { }
        protected virtual void DisposeListeners()
        {
            _disposables?.Dispose();
        }

        public void Dispose()
        {
            DisposeContainers();
            DisposeListeners();
        }
    }
}
