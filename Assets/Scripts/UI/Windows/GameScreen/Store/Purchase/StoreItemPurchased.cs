using Core.Context;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Store.Purchase
{
    public class StoreItemPurchased : Page
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text itemName;

        public override void Show(object ctx = null)
        {
            var info = ctx.Value<GoodInfo>();
            
            icon.sprite = info.SquareImage;
            itemName.text = info.Name;
            
            base.Show(ctx);
        }
    }
}