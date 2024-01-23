using Game.Pages;
using UI.Windows.Pages;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameScreen
{
    /// <summary>
    /// Страница описания основных характеристик
    /// </summary>
    public class StatsDescriptionPage : Page
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text statName;
        [SerializeField] private Text statDesc;

        /// <summary>
        /// Показываент описание основной характеристики
        /// </summary>
        public void Show(Sprite sprite, string nameKey, string descKey)
        {
            icon.sprite = sprite;
            statName.text = GetLocale(nameKey).ToUpper();
            statDesc.text = GetLocale(descKey);

            Open();
        }
    }
}