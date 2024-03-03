using Core;
using Extensions;
using UI.Models;
using UnityEngine;

namespace UI.GameScreen
{
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
        
        public static void SetActive(bool value)
        {
            Instance._canvasGroup.Set(value ? Instance.shownSettings : Instance.hiddenSettings);
        }
    }
    
}