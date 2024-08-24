using Core;
using Core.Context;
using MessageBroker;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using UI.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Store.Purchase
{
    public class StoreItemPurchased : Page
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text itemName;
        [SerializeField] private Button okButton;

        private int _category;

        private void Start()
        {
            okButton.onClick.AddListener(ReturnToShop);
        }

        public override void Show(object ctx = null)
        {
            var info = ctx.ValueByKey<GoodInfo>("item_info");
            _category = ctx.ValueByKey<int>("category");

            icon.sprite = info.SquareImage;
            itemName.text = info.Name;

            base.Show(ctx);
        }

        private void ReturnToShop()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.Shop, _category));
        }
    }
}