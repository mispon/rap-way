using UI.Base.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Base
{
    [RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
    public class OverlayBlackout : MonoBehaviour, IUIElement
    {
        private GraphicRaycaster _graphicRaycaster;
        private Canvas _canvas;

        private void Awake()
        {
            _graphicRaycaster = GetComponent<GraphicRaycaster>();
            _canvas = GetComponent<Canvas>();
            _graphicRaycaster.enabled = false;
        }

        public void Show(object ctx)
        {
            _graphicRaycaster.enabled = true;
            _canvas.enabled = true;
        }

        public void Hide()
        {
            _graphicRaycaster.enabled = false;
            _canvas.enabled = false;
        }

        public void Dispose() {}
    }
}