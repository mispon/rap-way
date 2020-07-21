using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Game.Pages.Rappers
{
    /// <summary>
    /// Страница со списком всех существующих исполнителей
    /// </summary>
    public class RappersListPage : Page
    {
        [Header("Контролы и настройки")]
        [SerializeField] private RectTransform listContainer;
        [SerializeField] private RapperItem rapperItemTemplate;
        [SerializeField] private int rapperItemSize = 250;

        [Header("Персональная страница")]
        [SerializeField] private RapperPage rapperPage;
        
        [Header("Данные об исполнителях")]
        [SerializeField] private RappersData data;

        private readonly List<RapperItem> _rappersList = new List<RapperItem>();
        
        /// <summary>
        /// Обработчик нажатия на элемент списка
        /// </summary>
        private void HandleItemClick(RapperItem item)
        {
            rapperPage.OpenPage(item.Info);
        }

        /// <summary>
        /// Создает элемент списка реперов
        /// </summary>
        private void CreateItem(RapperInfo info)
        {
            var rapperItem = Instantiate(rapperItemTemplate, listContainer);
                
            rapperItem.Setup(info);
            rapperItem.onClick += HandleItemClick;
            rapperItem.gameObject.SetActive(true);
                
            _rappersList.Add(rapperItem);
        }
        
        /// <summary>
        /// Переопределение размера контейнера
        /// </summary>
        private void Resize()
        {
            listContainer.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical,
                rapperItemSize * _rappersList.Count
            );
        }

        #region PAGE CALLBACKS

        protected override void BeforePageOpen()
        {
            if (_rappersList.Count > 0)
                return;
            
            foreach (var rapperInfo in data.Rappers)
                CreateItem(rapperInfo);

            Resize();
        }

        private void OnDestroy()
        {
            foreach (var rapperItem in _rappersList)
            {
                rapperItem.onClick -= HandleItemClick;
            }
            
            _rappersList.Clear();
        }

        #endregion
    }
}