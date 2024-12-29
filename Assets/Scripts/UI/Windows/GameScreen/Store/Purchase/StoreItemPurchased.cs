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
            var info = ctx.ValueByKey<StoreItemInfo>("item_info");
            _category = ctx.ValueByKey<int>("category");

            icon.sprite   = info.SquareImage;
            itemName.text = info.Name;

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