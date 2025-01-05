using Core;
using Core.Analytics;
using Core.Context;
using Enums;
using MessageBroker;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using UI.Enums;
using UnityEngine;
using UnityEngine.UI;
using StoreItemInfo = ScriptableObjects.StoreItem;
using ClothingItemInfo = ScriptableObjects.ClothingItem;

namespace UI.Windows.GameScreen.Store.Purchase
{
    public class StoreItemPurchased : Page
    {
        [SerializeField] private Image  icon;
        [SerializeField] private Text   itemName;
        [SerializeField] private Button okButton;

        private int _category;

        private void Start()
        {
            okButton.onClick.AddListener(ReturnToShop);
        }

        public override void Show(object ctx = null)
        {
            _category = ctx.ValueByKey<int>("category");

            var shopItem = ctx.ValueByKey<StoreItemInfo>("item_info");
            if (shopItem != null)
            {
                icon.sprite   = shopItem.SquareImage;
                itemName.text = shopItem.Name;
            }

            var clothingItem = ctx.ValueByKey<ClothingItemInfo>("clothing_item_info");
            if (clothingItem != null)
            {
                icon.sprite   = clothingItem.SquareImage;
                itemName.text = clothingItem.Name;
            }

            base.Show(ctx);
        }

        protected override void AfterShow(object ctx = null)
        {
            AnalyticsManager.LogEvent(FirebaseGameEvents.ShopItemPurchased);
            base.AfterShow(ctx);
        }

        private void ReturnToShop()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.Shop, _category));
        }
    }
}