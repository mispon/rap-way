using System.Collections.Generic;
using Core;
using Data;
using Firebase.Analytics;
using Game.UI.ScrollViewController;
using UnityEngine;

namespace Game.Pages.Store
{
    public class StorePage: Page
    {
        [SerializeField] private GoodsData data;
        
        [Space, Header("Categories")]
        [SerializeField] private ScrollViewController categories;
        [SerializeField] private GameObject categoryItemTemplate;

        private readonly List<StoreCategoryItem> _categoryItems = new();
        
        protected override void BeforePageOpen()
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.ShopOpened);

            int i = 1;
            foreach (var goodInfo in data.Items)
            {
                var row = categories.InstantiatedElement<StoreCategoryItem>(categoryItemTemplate);
                
                row.Initialize(i, goodInfo);
                if (i == 1)
                {
                    row.ShowItems();
                }
                i++;
                
                _categoryItems.Add(row);
            }
            
            categories.RepositionElements(_categoryItems);
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