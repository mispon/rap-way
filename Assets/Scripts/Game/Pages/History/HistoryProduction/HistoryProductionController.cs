using System;
using System.Collections.Generic;
using Core;
using Game.UI.ScrollViewController;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.Serialization;
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
        private ScrollViewController _scrollViewController;
        
        /// <summary>
        /// Список всех уже созданных UI-элементов экземпляров Production
        /// </summary>
        [FormerlySerializedAs("generatedItemsList")]
        public List<HistoryRow> historyRows = new List<HistoryRow>();
        
        /// <summary>
        /// Функция получения массива всех созданных игроком экземпляров Production
        /// </summary>
        protected virtual Production[] PlayerProductionInfos() => new Production[0];
        
        /// <summary>
        /// Инициализация контроллера. Сохранение зависимостей и подписка на события выбора типа Production
        /// </summary>
        public void Initialize(HistoryPage historyPage, ScrollViewController scrollViewController)
        {
            if(_isInitialized)
                return;
            
            _historyPage = historyPage;
            _scrollViewController = scrollViewController;
            
            _historyPage.onFoldoutInfoShow += SetActiveUi;
            foldoutBtn.onClick.AddListener(() => Show());
            
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
        public void Show(bool silent = false)
        {
            if (!silent)
                SoundManager.Instance.PlaySwitch();
            
            _historyPage.ShowInfo(this);

            var productionInfo = PlayerProductionInfos();
            if (!CheckAnyProduction(in productionInfo))
                return;

            if (!ProductionIsUpdated(in productionInfo))
            {
                _scrollViewController.RepositionElements(historyRows);
                return;
            }
            
            GenerateNewItems(in productionInfo);
        }

        /// <summary>
        /// Деактивация объектов-информации об экземлярах Production
        /// </summary>
        public void SetActiveElements(bool value)
        {
            foreach (var itemController in historyRows)
            {
                itemController.gameObject.SetActive(value);
            }
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
            => productionInfo.Length > historyRows.Count;

        /// <summary>
        /// Создаем новые UI-элементы экземпляров, обновляем порядковые номера старых, позиционируем UI-элементы
        /// </summary>
        private void GenerateNewItems(in Production[] productionInfo)
        {
            var newElementsCount = productionInfo.Length - historyRows.Count;
            foreach (var generatedItems in historyRows)
            {
                generatedItems.UpdateNum(newElementsCount);
            }

            for (int i = 0; i < newElementsCount; i++)
            {
                var row = _scrollViewController.InstantiatedElement<HistoryRow>(templateObject);
                row.Initialize(i + 1, productionInfo[i].HistoryInfo);
                historyRows.Add(row);
            }
            
            _scrollViewController.RepositionElements(historyRows);
        }
    }
}