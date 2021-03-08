using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.History
{
    /// <summary>
    /// Объект-информация об экземляре Production
    /// </summary>
    public class HistoryInfoItemController: MonoBehaviour
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

        /// <summary>
        /// Четность элемента в списке
        /// </summary>
        private bool isEven => _orderNumber % 2 == 0;
        
        /// <summary>
        /// Высота элемента
        /// </summary>
        public float Height { get; private set; }

        /// <summary>
        /// Заполнение информации об экземпляре при создании
        /// </summary>
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

        /// <summary>
        /// Обновление порядкового номера при добавлении новых записей в таблицу
        /// </summary>
        public void UpdateNum(int increment)
        {
            orderNumber += increment;
            SetupColor();
        }

        /// <summary>
        /// Устанавливает цвет записи
        /// </summary>
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

        /// <summary>
        /// Позиционирование элемента в таблице в соответсвии с его порядковым номером
        /// </summary>
        public void SetPosition(float spacing)
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
             
            if (Height == 0)
                Height = _rectTransform.rect.height;
            
            _rectTransform.anchoredPosition = Vector2.down * ((spacing * _orderNumber) + (Height * (_orderNumber-1)));
        }
    }
}