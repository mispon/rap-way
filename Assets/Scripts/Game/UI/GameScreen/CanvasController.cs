using Models.UI;
using UnityEngine;
using Utils;
using Utils.Extensions;

namespace Game.UI.GameScreen
{
    /// <summary>
    /// Управление канвасом
    /// </summary>
    public class CanvasController: Singleton<CanvasController>
    {
        [Header("Настройки отображения канваса")]
        [SerializeField] private CanvasGroupSettings shownSettings;
        [SerializeField] private CanvasGroupSettings hiddenSettings;

        private CanvasGroup _canvasGroup;
        
        protected override void Awake()
        {
            base.Awake();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            SetActive(true);
        }

        /// <summary>
        /// Изменяет видимость всех UI элементов
        /// </summary>
        public static void SetActive(bool value)
        {
            Instance._canvasGroup.Set(value ? Instance.shownSettings : Instance.hiddenSettings);
        }
    }
    
}