using Core.Localization;
using Extensions;
using Models.Eagler;
using UI.Controls.ScrollViewController;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Pages.Eagler
{
    public class EagleCard: MonoBehaviour, IScrollViewControllerItem
    {
        [Header("Eagle fields")]
        [SerializeField] private Text nickname;
        [SerializeField] private Text date;
        [SerializeField] private Text message;
        [SerializeField] private Text likes;
        [SerializeField] private Text views;
        [SerializeField] private Text shares;
        
        private RectTransform _rectTransform;
        
        private int _index { get; set; }
        private float _height { get; set; }
        private float _width { get; set; }
        
        public void Initialize(int i, Eagle eagle)
        {
            _index = i;
            nickname.text = $"@{eagle.Nickname}";
            date.text = eagle.Date;
            message.text = eagle.IsUser
                ? eagle.Message
                : $"{LocalizationManager.Instance.Get(eagle.Message)}{eagle.Tags}";
            likes.text = eagle.Likes.GetDisplay();
            views.text = eagle.Views.GetDisplay();
            shares.text = eagle.Shares.GetDisplay();
        }
        
        public void SetPosition(float spacing)
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
             
            if (_height == 0)
                _height = _rectTransform.rect.height;
            if (_width == 0)
                _width = _rectTransform.rect.width;
            
            var pos = Vector2.down * ((spacing * (_index-1)) + (_height * (_index-1)));
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
    }
}