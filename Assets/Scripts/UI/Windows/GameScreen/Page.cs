using Core.Localization;
using Game;
using Game.Settings;
using UI.Base;

namespace UI.Windows.GameScreen
{
    /// <summary>
    /// Базовый класс всех страниц игры
    /// TODO: replace Open/Close logic by CanvasUIElement
    /// </summary>
    public class Page : CanvasUIElement
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
            Show(new object());
            
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
            Hide();
            
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

        /// <summary>
        /// Обработчики событий открытия/закрытия страницы
        /// </summary>
        protected virtual void BeforePageOpen() {}
        protected virtual void AfterPageOpen() {}
        protected virtual void BeforePageClose() {}
        protected virtual void AfterPageClose() {}
    }
}