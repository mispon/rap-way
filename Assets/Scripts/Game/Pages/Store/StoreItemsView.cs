using System.Collections.Generic;
using Data;
using Game.UI.ScrollViewController;
using UnityEngine;

namespace Game.Pages.Store
{
    public class StoreItemsView : MonoBehaviour
    {
        [Space, Header("Items")]
        [SerializeField] private ScrollViewController items;
        [SerializeField] private GameObject itemTemplate;
        
        private readonly List<StoreItem> _storeItems = new();
        
        public void Show(GoodInfo info)
        {
            Clear();
            
            int i = 1;
            foreach (var itemInfo in info.UI)
            {
                var item = items.InstantiatedElement<StoreItem>(itemTemplate);
                
                item.Initialize(i, itemInfo);
                i++;
                
                _storeItems.Add(item);
            }
            
            items.RepositionElements(_storeItems);
        }

        private void Clear()
        {
            foreach (var item in _storeItems)
            {
                Destroy(item.gameObject);
            }
            _storeItems.Clear();
        }
    }
}