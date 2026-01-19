using Cysharp.Threading.Tasks;
using RapWay.Core.UI;
using RapWay.UI.Windows;
using UnityEngine;
using VContainer;

namespace RapWay.UI.Widgets
{
    public class NavigationButton : BaseButton
    {
        [Header("Navigation Settings")]
        [SerializeField] private BaseWindow targetWindowPrefab;
        
        private UIService _uiService;

        [Inject]
        public void Construct(UIService uiService)
        {
            _uiService = uiService;
        }

        protected void Start()
        {
            AddListener(Navigate);
        }

        private void Navigate()
        {
            if (targetWindowPrefab != null)
            {
                var targetType =  targetWindowPrefab.GetType();
                _uiService.Open(targetType).Forget();
            }
        }
    }
}