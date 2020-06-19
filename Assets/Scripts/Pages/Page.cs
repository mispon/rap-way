using UnityEngine;

namespace Pages
{
    /// <summary>
    /// Базовый класс всех страниц игры
    /// </summary>
    public class Page : MonoBehaviour
    {
        public bool IsOpen { get; private set; }

        /// <summary>
        /// Открывает страницу
        /// </summary>
        public void Open()
        {
            IsOpen = true;
            gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Закрывает страницу
        /// </summary>
        public void Close()
        {
            IsOpen = false;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Переключает состояние страницы
        /// </summary>
        public void Toggle()
        {
            IsOpen = !IsOpen;
            gameObject.SetActive(IsOpen);
        }
    }
}