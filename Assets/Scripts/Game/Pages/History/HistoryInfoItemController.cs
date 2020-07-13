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

        /// <summary>
        /// Порядоквый номер элемента в таблице (начаниется с 1)
        /// </summary>
        private int _num;
        /// <summary>
        /// Высота элемента
        /// </summary>
        public float Height { get; private set; }
        private RectTransform _rectTransform;

        /// <summary>
        /// Заполнение информации об экземпляре при создании
        /// </summary>
        public void Initialize(int num, string[] infos)
        {
            _num = num;
            numText.text = $"{_num}.";
            for (int i = 0; i < Mathf.Min(infoTexts.Length, infos.Length); i++)
                infoTexts[i].text = infos[i];
        }

        /// <summary>
        /// Обновление порядоквого номера при добавлении новых записей в таблицу
        /// </summary>
        public void UpdateNum(int increment)
        {
            _num += increment;
            numText.text = $"{_num}.";
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
            
            _rectTransform.anchoredPosition = Vector2.down * ((spacing * _num) + (Height * (_num-1)));
        }
    }
}