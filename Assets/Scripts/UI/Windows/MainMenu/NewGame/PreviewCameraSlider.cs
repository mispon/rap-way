using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.MainMenu.NewGame
{
    public class PreviewCameraSlider : MonoBehaviour
    {
        [SerializeField] private Camera previewCamera;
        [SerializeField] private Slider slider;

        private float initialPos;

        private void Start()
        {
            initialPos = previewCamera.transform.position.y;
            slider.onValueChanged.AddListener(HandleChange);
        }

        private void HandleChange(float val)
        {
            var newPos = previewCamera.transform.position;
            newPos.y = initialPos - val;

            previewCamera.transform.position = newPos;
        }
    }
}