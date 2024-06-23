using System;
using Core;
using ScriptableObjects;
using TMPro;
using UI.Controls.ScrollViewController;
using UnityEngine;
using UnityEngine.UI;
using EmailInfo = Game.SocialNetworks.Email.Email;

namespace UI.Windows.GameScreen.SocialNetworks.Email
{
    public class EmailCard : MonoBehaviour, IScrollViewControllerItem
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI preview;
        [SerializeField] private TextMeshProUGUI date;
        [SerializeField] private Image           border;

        [Space] [SerializeField] private Color selectedColor;
        [SerializeField]         private Color newColor;
        [SerializeField]         private Color oldColor;

        private EmailInfo                    _email;
        private Action<EmailCard, EmailInfo> _onClick;
        private RectTransform                _rectTransform;

        private int   _index  { get; set; }
        private float _height { get; set; }
        private float _width  { get; set; }

        public void SetPosition(float spacing)
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            if (_height == 0)
            {
                _height = _rectTransform.rect.height;
            }

            if (_width == 0)
            {
                _width = _rectTransform.rect.width;
            }

            var pos = Vector2.down * (spacing * (_index - 1) + _height * (_index - 1));
            _rectTransform.anchoredPosition = pos;
        }

        public float GetHeight()
        {
            return _height;
        }

        public float GetWidth()
        {
            return _width;
        }

        public void Unselect()
        {
            title.color  = oldColor;
            border.color = oldColor;
        }

        public void Initialize(int i, EmailInfo email, Action<EmailCard, EmailInfo> onClick)
        {
            _index = i;

            title.text   = email.Title;
            preview.text = email.Content;
            date.text    = email.Date;

            title.color  = email.IsNew ? newColor : oldColor;
            border.color = email.IsNew ? newColor : oldColor;

            _email   = email;
            _onClick = onClick;

            GetComponent<Button>().onClick.AddListener(HandleClick);
        }

        private void HandleClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);

            border.color = selectedColor;
            _onClick?.Invoke(this, _email);
        }
    }
}