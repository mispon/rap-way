using Game.UI.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    [RequireComponent(typeof(GraphicRaycaster), typeof(GraphicRaycaster))]
    public class OverlayBlackout : MonoBehaviour, IUIElement
    {
        private GraphicRaycaster _graphicRaycaster;

        private void Awake()
        {
            _graphicRaycaster = GetComponent<GraphicRaycaster>();
            _graphicRaycaster.enabled = false;
        }

        public void Show()
        {
            _graphicRaycaster.enabled = true;
        }

        public void Hide()
        {
            _graphicRaycaster.enabled = false;
        }
    }
}