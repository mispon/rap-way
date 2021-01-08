using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.MainMenu
{
    /// <summary>
    /// Контроллер окна "Об игре"
    /// </summary>
    public class AboutController : MonoBehaviour
    {
        [SerializeField] private Button okButton;

        private void Start()
        {
            okButton.onClick.AddListener(() => MainMenuController.SetPanelActivity(gameObject, false));
        }
    }
}