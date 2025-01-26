using System.Collections.Generic;
using Core;
using Core.Context;
using Extensions;
using Game.Player.Inventory.Desc;
using MessageBroker;
using MessageBroker.Messages.Player;
using MessageBroker.Messages.Player.State;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using UI.Controls.Error;
using UI.Enums;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using StoreItemInfo = ScriptableObjects.StoreItem;
using ClothingItemInfo = ScriptableObjects.ClothingItem;

namespace UI.Windows.GameScreen.Store.Purchase
{
    public class StoreItemPurchaseCard : Page
    {
        [Header("Error")]
        [SerializeField] private GameError gameError;

        [Space] [Header("Money Icon")]
        [SerializeField] private Sprite moneySprite;
        [SerializeField] private Sprite donateSprite;
        [SerializeField] private Sprite realMoneySprite;

        [Space] [Header("Card")]
        [SerializeField] private Image icon;
        [SerializeField] private Text   itemName;
        [SerializeField] private Text   itemDesc;
        [SerializeField] private Text   impact;
        [SerializeField] private Image  moneyIcon;
        [SerializeField] private Text   price;
        [SerializeField] private Button buyButton;
        [SerializeField] private Button cancelButton;

        private StoreItemInfo    _storeItem;
        private ClothingItemInfo _clothingItem;
        private int              _category;

        private readonly CompositeDisposable _disposable = new();

        private void Start()
        {
            buyButton.onClick.AddListener(BuyItemClick);
            cancelButton.onClick.AddListener(CloseCard);
        }

        public override void Show(object ctx = null)
        {
            _category = ctx.ValueByKey<int>("category");

            _storeItem = ctx.ValueByKey<StoreItemInfo>("item_info");
            if (_storeItem != null)
            {
                icon.sprite   = _storeItem.SquareImage;
                itemName.text = _storeItem.Name;

                var quality = _storeItem.Values.Quality * 100;
                itemDesc.text = !string.IsNullOrEmpty(_storeItem.Desc) ? GetLocale(_storeItem.Desc, quality) : "";

                const string impactTemplate = "<color=\"#ed8105\">Q: {0}%</color> | <color=\"#9c05ed\">H: {1}</color>";
                impact.text = string.Format(impactTemplate, quality, _storeItem.Values.Hype);

                moneyIcon.sprite = moneySprite;
                price.text       = _storeItem.Price.GetDisplay();
            }

            _clothingItem = ctx.ValueByKey<ClothingItemInfo>("clothing_item_info");
            if (_clothingItem != null)
            {
                icon.sprite   = _clothingItem.SquareImage;
                itemName.text = _clothingItem.Name;
                itemDesc.text = GetLocale("clothing_item_desc");

                impact.text = "";

                moneyIcon.sprite = moneySprite;
                price.text       = _clothingItem.Price.GetDisplay();
            }

            gameError.ForceHide();
            base.Show(ctx);
        }

        private void HandleItemPurchase(SpendMoneyResponse resp)
        {
            if (resp.Source != "store")
            {
                return;
            }

            if (!resp.OK)
            {
                gameError.Show(GetLocale("not_enough_money"));
                return;
            }

            SoundManager.Instance.PlaySound(UIActionType.Pay);

            if (_storeItem != null)
            {
                MsgBroker.Instance.Publish(new AddInventoryItemMessage
                {
                    Name = _storeItem.Name,
                    Type = _storeItem.Type,
                    Raw = new ValuesItem
                    {
                        Hype    = _storeItem.Values.Hype,
                        Quality = _storeItem.Values.Quality,
                        Level   = _storeItem.Values.Level
                    }
                });
            } else if (_clothingItem != null)
            {
                MsgBroker.Instance.Publish(new AddInventoryItemMessage
                {
                    Name = _clothingItem.Name,
                    Type = InventoryType.Clothes,
                    Raw  = _clothingItem.Value
                });
            }


            ShowPurchasedItem();
        }

        private void ShowPurchasedItem()
        {
            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type = WindowType.Shop_PurchasedItem,
                Context = new Dictionary<string, object>
                {
                    ["item_info"]          = _storeItem,
                    ["clothing_item_info"] = _clothingItem,
                    ["category"]           = _category
                }
            });
        }

        private void BuyItemClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            MsgBroker.Instance.Publish(new SpendMoneyRequest
            {
                Source = "store",
                Amount = _storeItem?.Price ?? _clothingItem.Price
            });
        }

        private void CloseCard()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.Shop, _category));
        }

        protected override void BeforeShow(object ctx = null)
        {
            MsgBroker.Instance
                .Receive<SpendMoneyResponse>()
                .Subscribe(HandleItemPurchase)
                .AddTo(_disposable);
        }

        protected override void AfterHide()
        {
            _disposable.Clear();
        }
    }
}