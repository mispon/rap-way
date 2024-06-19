using System;
using System.Collections.Generic;
using Core;
using Core.Context;
using Extensions;
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

namespace UI.Windows.GameScreen.Store.Purchase
{
    public class StoreItemPurchaseCard : Page
    {
        [Header("Purchaser"), SerializeField] private StoreItemPurchaser _purchaser;
        [Header("Error"), SerializeField] private GameError gameError;

        [Space, Header("Money Icon")] 
        [SerializeField] private Sprite moneySprite;
        [SerializeField] private Sprite donateSprite;
        [SerializeField] private Sprite realMoneySprite;

        [Space, Header("Card")]
        [SerializeField] private Image icon;
        [SerializeField] private Text itemName;
        [SerializeField] private Text itemDesc;
        [SerializeField] private Image moneyIcon;
        [SerializeField] private Text price;
        [SerializeField] private Button buyButton;
        [SerializeField] private Button cancelButton;

        private GoodInfo _info;
        private int _category;
        private readonly CompositeDisposable _disposable = new();
        
        private void Start()
        {
            buyButton.onClick.AddListener(BuyItemClick);
            cancelButton.onClick.AddListener(CloseCard);
        }
        
        public override void Show(object ctx = null)
        {
            _info     = ctx.ValueByKey<GoodInfo>("item_info");
            _category = ctx.ValueByKey<int>("category");
            
            icon.sprite = _info.SquareImage;
            itemName.text = _info.Name;
            itemDesc.text = !string.IsNullOrEmpty(_info.Desc) ? GetLocale(_info.Desc) : "";

            moneyIcon.sprite = GetMoneyIcon(_info);
            price.text = _info.Price.GetDisplay();
            
            gameError.ForceHide();
            base.Show(ctx);
        }

        private void HandleItemPurchase(bool ok)
        {
            if (!ok)
            {
                gameError.Show(GetLocale("not_enough_money"));
                return;
            }
            
            SoundManager.Instance.PlaySound(UIActionType.Pay);

            var newGoodEvent = StoreItemPurchaser.CreateNewGoodEvent(_info);
            MsgBroker.Instance.Publish(newGoodEvent);

            ShowPurchasedItem();
        }

        private void ShowPurchasedItem()
        {
            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type = WindowType.Shop_PurchasedItem,
                Context = new Dictionary<string, object>
                {
                    ["item_info"] = _info,
                    ["category"]  = _category
                }
            });
        }

        private void BuyItemClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            
            var itemType = StoreItemPurchaser.GetStoreItemType(_info);
            switch (itemType)
            {
                case StoreItemType.Donate:
                    MsgBroker.Instance.Publish(new SpendDonateRequest{Amount = _info.Price});
                    break;
                
                case StoreItemType.Game:
                    MsgBroker.Instance.Publish(new SpendMoneyRequest{Amount = _info.Price});    
                    break;
                
                case StoreItemType.Purchase:
                    _purchaser.PurchaseStoreItem(_info);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Sprite GetMoneyIcon(GoodInfo item)
        {
            return item switch
            {
                SwagGood  => moneySprite,
                EquipGood => moneySprite,
                
                DonateSwagGood  => donateSprite,
                DonateEquipGood => donateSprite,
                
                DonateCoins => realMoneySprite,
                NoAds       => realMoneySprite,
                
                _ => throw new ArgumentOutOfRangeException(nameof(item), item, null)
            };
        }
        
        private void CloseCard()
        {
            MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.Shop, _category));
        }
        
        protected override void BeforeShow(object ctx = null)
        {
            MsgBroker.Instance
                .Receive<SpendDonateResponse>()
                .Subscribe(e => HandleItemPurchase(e.OK))
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<SpendMoneyResponse>()
                .Subscribe(e => HandleItemPurchase(e.OK))
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<DonateAddedMessage>()
                .Subscribe(_ => ShowPurchasedItem())
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<NoAdsPurchaseMessage>()
                .Subscribe(_ => ShowPurchasedItem())
                .AddTo(_disposable);
        }

        protected override void AfterHide()
        {
            _disposable.Clear();
        }
    }
}