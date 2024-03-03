using UI.Controls.ScrollViewController;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.History
{
    public class HistoryRow: MonoBehaviour, IScrollViewControllerItem
    {
        [SerializeField] private Image panel;
        
        [Header("Поля данных")]
        [SerializeField] private Text numText;
        [SerializeField] private Text[] infoTexts;

        [Header("Цвета")]
        [SerializeField] private Color oddRowColor;
        [SerializeField] private Color evenRowColor;
        [SerializeField] private Color oddTextColor;
        [SerializeField] private Color evenTextColor;

        private int _orderNumber;
        private RectTransform _rectTransform;
        private int _itemsAmount;
        
        /// <summary>
        /// Порядоквый номер элемента в таблице (начаниется с 1)
        /// </summary>
        private int orderNumber
        {
            get => _orderNumber;
            set
            {
                _orderNumber = value;
                numText.text = $"{_orderNumber}";
            }
        }

        private bool isEven => _orderNumber % 2 == 0;

        private float _height { get; set; }
        private float _width { get; set; }
        
        public void Initialize(int index, string[] infos)
        {
            orderNumber = index;
            _itemsAmount = Mathf.Min(infoTexts.Length, infos.Length);

            for (int i = 0; i < _itemsAmount; i++)
            {
                infoTexts[i].text = infos[i];
            }

            SetupColor();
        }
        
        public void UpdateNum(int increment)
        {
            orderNumber += increment;
            SetupColor();
        }
        
        private void SetupColor()
        {
            Color panelColor = isEven ? evenRowColor : oddRowColor;
            Color textColor = isEven ? evenTextColor : oddTextColor;

            panel.color = panelColor;
            numText.color = textColor;
            for (int i = 0; i < _itemsAmount; i++)
            {
                infoTexts[i].color = textColor;
            }
        }
        
        public void SetPosition(float spacing)
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
             
            if (_height == 0)
                _height = _rectTransform.rect.height;
            if (_width == 0)
                _width = _rectTransform.rect.width;
            
            _rectTransform.anchoredPosition = Vector2.down * ((spacing * _orderNumber) + (_height * (_orderNumber-1)));
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