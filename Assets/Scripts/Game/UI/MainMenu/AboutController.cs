using Core;
using Data;
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
            okButton.onClick.AddListener(OnClose);
        }

        private void OnClose()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);            
            MainMenuController.SetPanelActivity(gameObject, false);
        }
    }
}