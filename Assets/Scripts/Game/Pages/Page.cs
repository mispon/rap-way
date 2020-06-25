using System;
using Localization;
using UnityEngine;
using Utils;

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
        /// Возвращает выбранное значение тематики или стиля 
        /// </summary>
        protected static T GetToneValue<T>(Switcher switcher) where T: Enum
        {
            var desc = LocalizationManager.Instance.GetKey(switcher.ActiveTextValue);
            return EnumExtension.GetFromDescription<T>(desc);
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