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
        [SerializeField] private TextMeshProUGUI sender;
        [SerializeField] private TextMeshProUGUI date;
        [SerializeField] private Image           image;
        [SerializeField] private Button          mainActionBtn;
        [SerializeField] private Button          quickActionBtn;

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
            sender.text  = email.Sender;
            date.text    = email.Date;

            mainActionBtn.gameObject.SetActive(email.IsNew && email.MainAction != null);
            quickActionBtn.gameObject.SetActive(email.IsNew && email.QuickAction != null);

            gameObject.SetActive(true);
        }

        public void ShowImage(EmailInfo email)
        {
            image.sprite = email.Sprite;
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