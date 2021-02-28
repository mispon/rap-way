using Localization;
using UnityEngine;

namespace Game.Pages
{
    /// <summary>
    /// Базовый класс всех страниц игры
    /// </summary>
    public class Page : MonoBehaviour
    {
        private bool _isOpen;

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
        /// Возвращает локализованную строку
        /// </summary>
        protected string GetLocale(string key, params object[] args)
        {
            return args.Length > 0
                ? LocalizationManager.Instance.GetFormat(key, args)
                : LocalizationManager.Instance.Get(key);
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