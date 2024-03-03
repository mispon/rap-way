using Core.Localization;
using Game;
using Game.Settings;
using UI.Base;

namespace UI.Windows.GameScreen
{
    /// <summary>
    /// Base class for all pages
    /// </summary>
    public class Page : CanvasUIElement
    {
        protected GameSettings settings => GameManager.Instance.Settings;

        /// <summary>
        /// Returns the localized string
        /// </summary>
        protected string GetLocale(string key, params object[] args)
        {
            return args.Length > 0
                ? LocalizationManager.Instance.GetFormat(key, args)
                : LocalizationManager.Instance.Get(key);
        }
    }
}