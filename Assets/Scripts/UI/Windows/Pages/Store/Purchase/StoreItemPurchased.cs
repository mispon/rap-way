using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Pages.Store.Purchase
{
    public class StoreItemPurchased : Page
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text itemName;

        public void Show(GoodInfo info)
        {
            icon.sprite = info.SquareImage;
            itemName.text = info.Name;
            
            Open();
        }
    }
}