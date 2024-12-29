using System.Collections.Generic;
using UI.Controls.ScrollViewController;
using UnityEngine;
using StoreItemInfo = ScriptableObjects.StoreItem;

namespace UI.Windows.GameScreen.Store
{
    public class StoreItemsView : MonoBehaviour
    {
        [SerializeField] private ScrollViewController items;
        [SerializeField] private GameObject           itemTemplate;

        private readonly List<StoreItem> _storeItems = new();

        public void Show(int category, StoreItemInfo[] storeItems)
        {
            Clear();

            var i = 1;
            foreach (var itemInfo in storeItems)
            {
                var item = items.InstantiatedElement<StoreItem>(itemTemplate);

                item.Initialize(i, category, itemInfo);
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