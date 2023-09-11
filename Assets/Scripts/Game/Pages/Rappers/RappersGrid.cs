using System.Collections.Generic;
using System.Linq;
using Core;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Rappers
{
    /// <summary>
    /// Страница со списком всех существующих исполнителей
    /// </summary>
    public class RappersGrid : MonoBehaviour
    {
        [Header("Контролы и настройки")]
        [SerializeField] private RectTransform listContainer;
        [SerializeField] private RapperGridItem rapperItemTemplate;

        [Header("Персональная карточка")]
        [SerializeField] private RapperCard rapperCard;

        [Header("Данные об исполнителях")]
        [SerializeField] private RappersData data;

        private readonly List<RapperGridItem> _rappersList = new();

        /// <summary>
        /// Инициализирует грид при первом вызове
        /// </summary>
        public void Init()
        {
            if (_rappersList.Count > 0)
                return;

            rapperCard.onDelete += HandleRapperDelete;
            
            foreach (var rapperInfo in data.Rappers)
            {
                CreateItem(rapperInfo);
            }
            foreach (var rapperInfo in GameManager.Instance.CustomRappers)
            {
                CreateItem(rapperInfo);
            }
        }
        
        /// <summary>
        /// Создает элемент списка реперов
        /// </summary>
        public void CreateItem(RapperInfo info)
        {
            var rapperItem = Instantiate(rapperItemTemplate, listContainer);
                
            rapperItem.Setup(info);
            rapperItem.onClick += HandleItemClick;
            rapperItem.gameObject.SetActive(true);
                
            _rappersList.Add(rapperItem);
        }
        
        /// <summary>
        /// Обработчик нажатия на элемент списка
        /// </summary>
        private void HandleItemClick(RapperGridItem item)
        {
            SoundManager.Instance.PlayClick();
            rapperCard.Show(item.Info);
        }

        /// <summary>
        /// Обрабатывает удаление кастомного реппера
        /// </summary>
        private void HandleRapperDelete(RapperInfo customRapper)
        {
            GameManager.Instance.CustomRappers.Remove(customRapper);
            
            var rapperItem = _rappersList.FirstOrDefault(r => r.Info.Id == customRapper.Id);
            if (rapperItem != null)
            {
                _rappersList.Remove(rapperItem);
                Destroy(rapperItem.gameObject);
            }
        }

        private void OnDestroy()
        {
            foreach (var rapperItem in _rappersList)
            {
                rapperItem.onClick -= HandleItemClick;
            }
            
            _rappersList.Clear();
            rapperCard.onDelete -= HandleRapperDelete;
        }
    }
}