using System;
using Sirenix.OdinInspector;
using UI.Base.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Base
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup), typeof(GraphicRaycaster))]
    public abstract class CanvasUIElement : SerializedMonoBehaviour, IUIElement, IDisposable
    {
        private Canvas _canvas;
        private Canvas canvas
        {
            get
            {
                if (_canvas == null)
                {
                    _canvas = GetComponent<Canvas>();
                }
                return _canvas;
            }
        }
        
        private CanvasGroup _canvasGroup;

        private CanvasGroup canvasGroup
        {
            get
            {
                if (_canvasGroup == null)
                {
                    _canvasGroup = GetComponent<CanvasGroup>();
                }
                return _canvasGroup;
            }
        }
        
        private bool _isActive;

        public virtual void Initialize()
        {
            Hide();
            SetupListenersOnInitialize();
        }
        
        public virtual void Show()
        {
            if (_isActive) return;
            
            _isActive = true;
            canvasGroup.interactable = true;
            canvas.enabled = true;
            
            SetupListenersOnShow();
            SendRequests();
        }
        
        public virtual void Hide()
        {
            if (!_isActive) return;
            
            _isActive = false;
            canvasGroup.interactable = false;
            canvas.enabled = false;
            
            DisposeContainers();
            DisposeListeners();
        }
        
        public void Dispose()
        {
            DisposeContainers();
            DisposeListeners();
        }
        
        protected virtual void SetupListenersOnInitialize() { }
        protected virtual void SetupListenersOnShow() { }
        protected virtual void SendRequests() { }
        protected virtual void DisposeContainers() { }
        protected virtual void DisposeListeners() {}
    }
}
