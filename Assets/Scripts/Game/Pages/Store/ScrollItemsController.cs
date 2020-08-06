using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Enums;
using Game.Effects;
using Game.Notifications;
using Models.Player;
using UnityEngine;

namespace Game.Pages.Store
{
    /// <summary>
    /// Класс управления UI-элементами товаров в ScrollView
    /// </summary>
    public class ScrollItemsController: MonoBehaviour
    {
        [Header("Элементы ScrollView")]
        [SerializeField] private RectTransform rectContent; 
        [Space, SerializeField] private GameObject elementTemplate;
        [SerializeField] private GameObject noElementsText;

        /// <summary>
        /// Массив данных об UI товаров для каждого типа шмотки каждого уровня
        /// </summary>
        private GoodInfo[] _goodInfos;
        
        /// <summary>
        /// Список контроллеров управления UI-элементами товара
        /// </summary>
        private readonly List<StoreItemController> _itemsList = new List<StoreItemController>();

        private float _elementTemplateHeight;
        private float _containerBaseHeight;
        private NewItemEffect _newGoodEffect;

        /// <summary>
        /// Высота одного элемента
        /// </summary>
        private float elementTemplateHeight
        {
            get
            {
                if (Math.Abs(_elementTemplateHeight) < 0.01f)
                    _elementTemplateHeight = elementTemplate.transform.GetComponent<RectTransform>().rect.height;

                return _elementTemplateHeight;
            }
        }
        /// <summary>
        /// Высота ViewPort'a
        /// </summary>
        private float containerBaseHeight
        {
            get
            {
                if (Math.Abs(_containerBaseHeight) < 0.01f)
                    _containerBaseHeight = rectContent.parent.GetComponent<RectTransform>().rect.height;

                return _containerBaseHeight;
            }
        }

        /// <summary>
        /// Инцниализация всех UI-элементов из GoodsData конкретного назначения (рабочие/понты)
        /// </summary>
        public void Initialize(GoodInfo[] goodInfos, NewItemEffect newGoodEffect)
        {
            _goodInfos = goodInfos;
            _newGoodEffect = newGoodEffect;
            
            foreach (var info in _goodInfos)
            {
                var newItemLevel = 1;
                var newItem = PlayerManager.Data.Goods.FirstOrDefault(g => g.Type == info.Type);
                if(newItem != default)
                    newItemLevel = newItem.Level + 1;
                
                if(info.UI.Any(el => el.Level == newItemLevel))
                    DrawItem(info.Type, (short)newItemLevel);
            }
            ResizeContainer();
        }

        /// <summary>
        /// Отрисовка элемента (предварительное создание контроллера при необходимости)
        /// </summary>
        /// <param name="nextLevel">Отрисовываем элемент следующего уровня?</param>
        private void DrawItem(GoodsType type, short level, bool nextLevel = false)
        {
            var itemController = _itemsList.FirstOrDefault(it => it.Type == type && it.Level == level);
            if (itemController == default)
            {
                var newItemObject = Instantiate(elementTemplate, rectContent);
                newItemObject.SetActive(true);

                itemController = newItemObject.GetComponent<StoreItemController>();
                _itemsList.Add(itemController);
            }


            var uiData = GetGoodUi(type, nextLevel ? level + 1 : level); 
            itemController.Initialize(type, uiData, OnPurchaseItemClick);
        }

        /// <summary>
        /// Возвращает данные отрисовки по типу и уровню шмотки
        /// </summary>
        private GoodUI GetGoodUi(GoodsType type, int level)
        {
            return _goodInfos.First(gi => gi.Type == type)
                .UI.First(el => el.Level == level); 
        }

        /// <summary>
        /// Событие нажатия кнопки "Купить"
        /// </summary>
        private void OnPurchaseItemClick(GoodsType type, short level, int price)
        {
            if (!PlayerManager.Instance.SpendMoney(price))
            {
                //todo: какая-то оповещалка, что "недостаточно средств"
                print("Не хватает золота!");
                return;
            }

            var good = PlayerManager.Data.Goods.FirstOrDefault(g => g.Type == type);
            if (good == null)
            {
                good = new Good {Type = type};
                PlayerManager.Data.Goods.Add(good);
            }
            good.Level = level;

            void Notification()
            {
                var uIData = GetGoodUi(good.Type, good.Level);
                Debug.LogWarning($"Показ шмотки {good.Type}_{good.Level}. Кликни в центр.");
                _newGoodEffect.Show(uIData.Image, IndependentNotificationManager.Instance.Unlock);    
            }
            
            IndependentNotificationManager.Instance.AddNotification(Notification);
            
            var info = _goodInfos.First(gi => gi.Type == type);
            if (level + 1 > info.MaxItemLevel)
                DisposeItem(type, level);
            else
                DrawItem(type, level, true);
        }

        /// <summary>
        /// Удаление контроллера UI-элементов товара по типу и уровню
        /// </summary>
        private void DisposeItem(GoodsType type, short level, bool resizeContainer = true)
        {
            var itemController = _itemsList.FirstOrDefault(it => it.Type == type && it.Level == level);
            if (itemController == default)
                return;

            DisposeItem(itemController, resizeContainer);
        }

        /// <summary>
        /// Удаление контроллера UI-элементов товара
        /// </summary>
        private void DisposeItem(StoreItemController itemController, bool resizeContainer = true)
        {
            _itemsList.Remove(itemController);
            Destroy(itemController.gameObject);
            if(resizeContainer)
                ResizeContainer();
        }
        
        /// <summary>
        /// Переопределение высоты контейнера ScrollView-элементов
        /// </summary>
        private void ResizeContainer()
        {
            bool anyElements = _itemsList.Count > 0;
            noElementsText.SetActive(!anyElements);

            rectContent.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical,
                anyElements
                    ? (elementTemplateHeight + 10) * _itemsList.Count
                    : containerBaseHeight
            );
        }

        /// <summary>
        /// Удаление всех элементов из ScrollView
        /// </summary>
        public void Dispose()
        {
            foreach (var itemController in _itemsList.ToArray())
                DisposeItem(itemController, false);

            _newGoodEffect = null;
        }
    }
}