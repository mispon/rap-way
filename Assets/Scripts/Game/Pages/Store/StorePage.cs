using System.Collections.Generic;
using Core;
using Data;
using Firebase.Analytics;
using Game.UI.ScrollViewController;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils.Extensions;

namespace Game.Pages.Store
{
    public class StorePage: Page
    {
        [BoxGroup("Data")] [SerializeField] private GoodsData data;
        
        [BoxGroup("Categories")] [SerializeField] private ScrollViewController categories;
        [BoxGroup("Categories")] [SerializeField] private GameObject categoryItemTemplate;

        private readonly List<StoreCategoryItem> _categoryItems = new();
        
        protected override void BeforePageOpen()
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.ShopOpened);

            int i = 1;
            foreach (var (type, itemsInfo) in data.Items)
            {
                var row = categories.InstantiatedElement<StoreCategoryItem>(categoryItemTemplate);

                var categoryIcon = data.Categories[type];
                row.Initialize(i, categoryIcon, GetLocale(type.GetDescription()), itemsInfo);
                i++;
                
                _categoryItems.Add(row);
            }
            
            categories.RepositionElements(_categoryItems);
        }

        protected override void AfterPageOpen()
        {
            _categoryItems[0].ShowItems();
        }

        protected override void AfterPageClose()
        {
            foreach (var item in _categoryItems)
            {
                Destroy(item.gameObject);
            }
            _categoryItems.Clear();
        }
    }
}