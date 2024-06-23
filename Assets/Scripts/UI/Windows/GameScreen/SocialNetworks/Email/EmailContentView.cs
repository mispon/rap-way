using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EmailInfo = Game.SocialNetworks.Email.Email;

namespace UI.Windows.GameScreen.SocialNetworks.Email
{
    public class EmailContentView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI content;
        [SerializeField] private Image           image;
        [SerializeField] private Button          mainActionBtn;
        [SerializeField] private Button          quickActionBtn;
        [SerializeField] private ImagesBank      imageBank;

        private EmailInfo _email;

        private void Start()
        {
            mainActionBtn.onClick.AddListener(HandleMainAction);
            quickActionBtn.onClick.AddListener(HandleQuickAction);
        }

        public void ShowText(EmailInfo email)
        {
            _email = email;

            title.text   = email.Title;
            content.text = email.Content;

            mainActionBtn.gameObject.SetActive(email.IsNew && email.MainAction != null);
            quickActionBtn.gameObject.SetActive(email.IsNew && email.QuickAction != null);

            gameObject.SetActive(true);
        }

        public void ShowImage(EmailInfo email)
        {
            image.sprite = imageBank.GetImageByName(email.SpriteName);

            ShowText(email);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void HandleMainAction()
        {
            _email.MainAction?.Invoke();
        }

        private void HandleQuickAction()
        {
            _email.QuickAction?.Invoke();
        }
    }
}