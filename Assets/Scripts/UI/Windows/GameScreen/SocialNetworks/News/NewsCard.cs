using Core.Localization;
using TMPro;
using UI.Controls.ScrollViewController;
using UnityEngine;
using NewsInfo = Game.SocialNetworks.News.News;

namespace UI.Windows.GameScreen.SocialNetworks.News
{
    public class NewsCard : MonoBehaviour, IScrollViewControllerItem
    {
        [SerializeField] private TextMeshProUGUI content;

        private RectTransform _rectTransform;

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

        public void Initialize(int i, NewsInfo news)
        {
            _index       = i;
            content.text = $"<b>{news.Date}:</b> {LocalizationManager.Instance.GetFormat(news.Text, news.TextArgs)}";
        }
    }
}