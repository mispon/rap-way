using Core.Localization;
using ScriptableObjects;
using TMPro;
using UI.Controls.ScrollViewController;
using UnityEngine;
using UnityEngine.UI;
using NewsInfo = Game.SocialNetworks.News.News;

namespace UI.Windows.GameScreen.SocialNetworks.News
{
    public class HotNewsCard : MonoBehaviour, IScrollViewControllerItem
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI content;
        [SerializeField] private TextMeshProUGUI date;
        [SerializeField] private ImagesBank imagesBank;

        private RectTransform _rectTransform;

        private int _index { get; set; }
        private float _height { get; set; }
        private float _width { get; set; }

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
            _index = i;

            image.sprite = news.Sprite != null ? news.Sprite : imagesBank.NoImage;
            content.text = LocalizationManager.Instance.GetFormat(news.Text, news.TextArgs);
            date.text = news.Date;
        }
    }
}