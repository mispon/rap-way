﻿using Sirenix.OdinInspector;
using UI.Base.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Base
{
    [RequireComponent(
        typeof(Canvas), 
        typeof(CanvasGroup), 
        typeof(GraphicRaycaster)
    )]
    public abstract class CanvasUIElement : SerializedMonoBehaviour, IUIElement
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
        
        public virtual void Show(object context)
        {
            if (_isActive) return;
            
            BeforeShow();
            
            _isActive = true;
            canvasGroup.interactable = true;
            canvas.enabled = true;
            
            AfterShow();
        }
        
        public virtual void Hide()
        {
            if (!_isActive) return;
            
            BeforeHide();
            
            _isActive = false;
            canvasGroup.interactable = false;
            canvas.enabled = false;

            AfterHide();
        }
        
        public void Dispose()
        {
            BeforeHide();
            AfterHide();
        }
        
        public    virtual void Initialize() {}
        protected virtual void BeforeShow() {}
        protected virtual void AfterShow() {}
        protected virtual void BeforeHide() {}
        protected virtual void AfterHide() {}
    }
}
