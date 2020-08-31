using Models.UI;
using UnityEngine;
using Utils;

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
            Instance.Set(value ? Instance.shownSettings : Instance.hiddenSettings);
        }

        /// <summary>
        /// Устанавливает параметры для CanvasGroup 
        /// </summary>
        private void Set(CanvasGroupSettings settings)
        {
            _canvasGroup.alpha = settings.alpha;
            _canvasGroup.interactable = settings.interactable;
            _canvasGroup.blocksRaycasts = settings.blocksRaycasts;
            _canvasGroup.ignoreParentGroups = settings.ignoreParentGroups;
        }
    }
    
}