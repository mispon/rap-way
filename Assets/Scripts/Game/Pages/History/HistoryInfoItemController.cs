using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.History
{
    /// <summary>
    /// Объект-информация об экземляре Production
    /// </summary>
    public class HistoryInfoItemController: MonoBehaviour
    {
        [Header("Поля данных")]
        [SerializeField] private Text numText;
        [SerializeField] private Text[] infoTexts;

        private int _orderNumber;
        private RectTransform _rectTransform;
        
        /// <summary>
        /// Порядоквый номер элемента в таблице (начаниется с 1)
        /// </summary>
        private int orderNumber
        {
            get => _orderNumber;
            set
            {
                _orderNumber = value;
                numText.text = $"{_orderNumber}.";
            }
        }
        
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
            for (int i = 0; i < Mathf.Min(infoTexts.Length, infos.Length); i++)
                infoTexts[i].text = infos[i];
        }

        /// <summary>
        /// Обновление порядоквого номера при добавлении новых записей в таблицу
        /// </summary>
        public void UpdateNum(int increment)
        {
            orderNumber += increment;
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