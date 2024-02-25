using System;
using Core;
using Extensions;
using MessageBroker;
using MessageBroker.Messages.Player;
using MessageBroker.Messages.Player.State;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UI.Controls.Error;
using UI.Windows.GameScreen;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Pages.Store.Purchase
{
    public class StoreItemPurchaseCard : Page
    {
        private readonly CompositeDisposable _disposable = new();
        
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
        [Space]
        [SerializeField] private StoreItemPurchased purchasedItem;
        
        private GoodInfo _info;
        
        private void Start()
        {
            buyButton.onClick.AddListener(BuyItemClick);
        }

        public void Show(GoodInfo info)
        {
            _info = info;
            
            icon.sprite = info.SquareImage;
            itemName.text = info.Name;
            itemDesc.text = !string.IsNullOrEmpty(info.Desc) ? GetLocale(info.Desc) : "";

            moneyIcon.sprite = GetMoneyIcon(info);
            price.text = info.Price.GetDisplay();
            
            gameError.ForceHide();
            Open();
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
            
            purchasedItem.Show(_info);
            Close();
        }

        private void HandleDonatePurchase()
        {
            purchasedItem.Show(_info);
            Close();
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
        
        protected override void BeforePageOpen()
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
                .Subscribe(_ => HandleDonatePurchase())
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<NoAdsPurchaseMessage>()
                .Subscribe(_ => HandleDonatePurchase())
                .AddTo(_disposable);
        }

        protected override void AfterPageClose()
        {
            _disposable.Clear();
        }
    }
}