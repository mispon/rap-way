using System;
using Core;
using Core.Context;
using Extensions;
using MessageBroker;
using MessageBroker.Messages.Player;
using MessageBroker.Messages.Player.State;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UI.Controls.Error;
using UI.Enums;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Store.Purchase
{
    public class StoreItemPurchaseCard : Page
    {
        [BoxGroup("Purchaser")] [SerializeField] private StoreItemPurchaser _purchaser;
        [BoxGroup("Error")] [SerializeField] private GameError gameError;
        [Space]
        [BoxGroup("Money Icon")] [SerializeField] private Sprite moneySprite;
        [BoxGroup("Money Icon")] [SerializeField] private Sprite donateSprite;
        [BoxGroup("Money Icon")] [SerializeField] private Sprite realMoneySprite;
        [Space]
        [BoxGroup("Card")] [SerializeField] private Image icon;
        [BoxGroup("Card")] [SerializeField] private Text itemName;
        [BoxGroup("Card")] [SerializeField] private Text itemDesc;
        [BoxGroup("Card")] [SerializeField] private Image moneyIcon;
        [BoxGroup("Card")] [SerializeField] private Text price;
        [BoxGroup("Card")] [SerializeField] private Button buyButton;

        private GoodInfo _info;
        private readonly CompositeDisposable _disposable = new();
        
        private void Start()
        {
            buyButton.onClick.AddListener(BuyItemClick);
        }

        public override void Show(object ctx = null)
        {
            _info = ctx.Value<GoodInfo>();
            
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
                Context = _info
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