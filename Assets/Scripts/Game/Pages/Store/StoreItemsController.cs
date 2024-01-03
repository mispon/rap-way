using System.Collections.Generic;
using System.Linq;
using Core;
using Data;
using Enums;
using Firebase.Analytics;
using Game.Effects;
using Game.Notifications;
using Game.UI;
using Models.Player;
using UnityEngine;

namespace Game.Pages.Store
{
    /// <summary>
    /// Класс управления UI-элементами товаров в ScrollView
    /// </summary>
    public class StoreItemsController: MonoBehaviour
    {
        [Header("Элементы ScrollView")]
        [SerializeField] private RectTransform rectContent; 
        [Space, SerializeField] private GameObject elementTemplate;
        [SerializeField] private GameObject noElementsText;
        [Space]
        [SerializeField] private StoreItemInfoPage infoPage;

        /// <summary>
        /// Массив данных об UI товаров для каждого типа шмотки каждого уровня
        /// </summary>
        private GoodInfo[] _goodInfos;
        
        /// <summary>
        /// Список контроллеров управления UI-элементами товара
        /// </summary>
        private readonly List<StoreItemController> _itemsList = new List<StoreItemController>();

        private NewItemEffect _newGoodEffect;

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
                if (newItem != default)
                {
                    newItemLevel = newItem.Level + 1;
                }

                if (info.UI.Any(el => el.Level == newItemLevel))
                {
                    DrawItem(info.Type, (short) newItemLevel);
                }
            }

            ShowIfNoItems();
        }

        /// <summary>
        /// Отрисовка элемента (предварительное создание контроллера при необходимости)
        /// </summary>
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
            itemController.Initialize(type, uiData, OnPurchaseItemClick, OnItemClick);
        }

        /// <summary>
        /// Возвращает данные отрисовки по типу и уровню шмотки
        /// </summary>
        private GoodUI GetGoodUi(GoodsType type, int level)
        {
            return _goodInfos.First(gi => gi.Type == type).UI.First(el => el.Level == level);
        }

        /// <summary>
        /// Событие нажатия кнопки "Купить"
        /// </summary>
        private void OnPurchaseItemClick(GoodsType type, short level, int price, int hype, Price label)
        {
            if (!PlayerManager.Instance.SpendMoney(price))
            {
                label.ShowNoMoney();
                return;
            }
            
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.ShopItemPurchased);

            var good = PlayerManager.Data.Goods.FirstOrDefault(g => g.Type == type);
            if (good == null)
            {
                good = new Good {Type = type};
                PlayerManager.Data.Goods.Add(good);
            }
            good.Level = level;
            good.Hype = hype;

            void Notification()
            {
                var uIData = GetGoodUi(good.Type, good.Level);
                _newGoodEffect.Show(uIData.Image, NotificationManager.Instance.UnlockIndependentQueue);
            }
            
            NotificationManager.Instance.AddIndependentNotification(Notification);
            
            var info = _goodInfos.First(gi => gi.Type == type);
            if (level + 1 > info.MaxItemLevel)
            {
                DisposeItem(type, level);
            }
            else
            {
                DrawItem(type, level, true);
            }

            if (hype > 0)
            {
                // обновляем отображение хайпа
                PlayerManager.Instance.AddHype(0);
            }
        }

        /// <summary>
        /// Показывает детальную информацию о товаре
        /// </summary>
        private void OnItemClick(GoodsType type, short level, int price)
        {
            var uIData = GetGoodUi(type, level);
            infoPage.Show(uIData.Image, type, level, price, uIData.Hype);
        }

        /// <summary>
        /// Удаление контроллера UI-элементов товара по типу и уровню
        /// </summary>
        private void DisposeItem(GoodsType type, short level)
        {
            var itemController = _itemsList.FirstOrDefault(it => it.Type == type && it.Level == level);
            if (itemController == default)
            {
                return;
            }

            DisposeItem(itemController);
        }

        /// <summary>
        /// Удаление контроллера UI-элементов товара
        /// </summary>
        private void DisposeItem(StoreItemController itemController, bool resizeContainer = true)
        {
            _itemsList.Remove(itemController);
            Destroy(itemController.gameObject);
            ShowIfNoItems();
        }

        /// <summary>
        /// Показывает сообщение о том, что нет товаров
        /// </summary>
        private void ShowIfNoItems()
        {
            noElementsText.SetActive(_itemsList.Count == 0);
        }

        /// <summary>
        /// Удаление всех элементов из ScrollView
        /// </summary>
        public void Dispose()
        {
            foreach (var itemController in _itemsList.ToArray())
            {
                DisposeItem(itemController, false);
            }

            _newGoodEffect = null;
        }
    }
}