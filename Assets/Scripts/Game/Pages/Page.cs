using System;
using System.Collections.Generic;
using Core.Settings;
using Localization;
using UniRx;
using UnityEngine;

namespace Game.Pages
{
    /// <summary>
    /// Базовый класс всех страниц игры
    /// </summary>
    public class Page : MonoBehaviour
    {
        private bool _isOpen;

        protected GameSettings settings => GameManager.Instance.Settings;

        /// <summary>
        /// Checks page open or not 
        /// </summary>
        public bool IsOpen() => _isOpen;
        
        /// <summary>
        /// Открывает страницу
        /// </summary>
        public void Open()
        {
            if (_isOpen) return;
            
            BeforePageOpen();
            _isOpen = true;
            gameObject.SetActive(true);
            AfterPageOpen();
        }
        
        /// <summary>
        /// Закрывает страницу
        /// </summary>
        public void Close()
        {
            if (!_isOpen) return;
            
            BeforePageClose();
            _isOpen = false;
            gameObject.SetActive(false);
            AfterPageClose();
        }

        /// <summary>
        /// Returns the localized string
        /// </summary>
        protected string GetLocale(string key, params object[] args)
        {
            return args.Length > 0
                ? LocalizationManager.Instance.GetFormat(key, args)
                : LocalizationManager.Instance.Get(key);
        }

        protected void SendMessage<T>(T msg) where T: struct
        {
            GameManager.Instance.MessageBroker.Publish(msg);
        }
        
        protected void RecvMessage<T>(Action<T> handler, ICollection<IDisposable> disposable) where T: struct
        {
            GameManager.Instance.MessageBroker
                .Receive<T>()
                .Subscribe(handler)
                .AddTo(disposable);
        }

        /// <summary>
        /// Обработчики событий открытия/закрытия страницы
        /// </summary>
        protected virtual void BeforePageOpen() {}
        protected virtual void AfterPageOpen() {}
        protected virtual void BeforePageClose() {}
        protected virtual void AfterPageClose() {}
    }
}