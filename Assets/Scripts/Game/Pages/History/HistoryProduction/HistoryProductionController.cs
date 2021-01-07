using System;
using System.Collections.Generic;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.History.HistoryProduction
{
    /// <summary>
    /// Контроллер UI-элементов. Обрабатывает команду на отображение информации по конкретному Production
    /// </summary>
    [Serializable]
    public class HistoryProductionController
    {
        [Header("Вкладка переключения")] 
        [SerializeField] private Button foldoutBtn;
        
        [Header("UI-элементы заголовков колонок")]
        [SerializeField] private GameObject headerColumnObject;
        
        [Header("UI-элементы шаблоны")]
        [SerializeField] private GameObject templateObject;

        private bool _isInitialized;
        private HistoryPage _historyPage;
        private HistoryScrollViewController _scrollViewController;
        
        /// <summary>
        /// Список всех уже созданных UI-элементов экземпляров Production
        /// </summary>
        public List<HistoryInfoItemController> generatedItemsList = new List<HistoryInfoItemController>();
        
        /// <summary>
        /// Функция получения массива всех созданных игроком экземпляров Production
        /// </summary>
        protected virtual Production[] PlayerProductionInfos() => new Production[0];
        
        /// <summary>
        /// Инициализация контроллера. Сохранение зависимостей и подписка на события выбора типа Production
        /// </summary>
        public void Initialize(HistoryPage historyPage, HistoryScrollViewController scrollViewController)
        {
            if(_isInitialized)
                return;
            
            _historyPage = historyPage;
            _scrollViewController = scrollViewController;
            
            _historyPage.onFoldoutInfoShow += SetActiveUi;
            foldoutBtn.onClick.AddListener(Show);
            
            _isInitialized = true;
        }

        /// <summary>
        /// Активация/Деактивация UI-элементов в шапке (кнопок-вкладок + заголовков таблицы)
        /// </summary>
        private void SetActiveUi(HistoryProductionController activeController)
        {
            bool active = activeController == this;
            foldoutBtn.interactable = !active;
            headerColumnObject.SetActive(active);
        }
        
        /// <summary>
        /// Выбор Production. Переключение со старого Production на новый с отрисовкой новых экзепляров
        /// </summary>
        public void Show()
        {
            _historyPage.ShowInfo(this);

            var productionInfo = PlayerProductionInfos();
            if (!CheckAnyProduction(in productionInfo))
                return;

            if (!ProductionIsUpdated(in productionInfo))
                return;

            GenerateNewItems(in productionInfo);
        }

        /// <summary>
        /// Деактивация объектов-информации об экземлярах Production
        /// </summary>
        public void SetActiveElements(bool value)
        {
            foreach (var itemController in generatedItemsList)
                itemController.gameObject.SetActive(value);
        }

        /// <summary>
        /// Проверка, есть ли какие-то экземпляры выбранного Production
        /// </summary>
        private bool CheckAnyProduction(in Production[] productionInfo)
        {
            bool anyElements = productionInfo.Length != 0;
            SetActiveElements(anyElements);
            return anyElements;
        }

        /// <summary>
        /// Проверка, есть ли новые экземпляры выбранного Production
        /// </summary>
        private bool ProductionIsUpdated(in Production[] productionInfo)
            => productionInfo.Length > generatedItemsList.Count;

        /// <summary>
        /// Создаем новые UI-элементы экземпляров, обновляем порядковые номера старых, позиционируем UI-элементы
        /// </summary>
        private void GenerateNewItems(in Production[] productionInfo)
        {
            var newElementsCount = productionInfo.Length - generatedItemsList.Count;
            foreach (var generatedItems in generatedItemsList)
                generatedItems.UpdateNum(newElementsCount);
            
            for (int i = 0; i < newElementsCount; i++)
            {
                var itemController = _scrollViewController.InstantiatedElement(templateObject);
                itemController.Initialize(i+1, productionInfo[i].HistoryInfo);
                generatedItemsList.Add(itemController);
            }
            _scrollViewController.RepositionElements(generatedItemsList);
        }
    }
}