using System;
using Core;
using Extensions;
using MessageBroker;
using MessageBroker.Messages.Donate;
using MessageBroker.Messages.State;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UI.Controls.Error;
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
        [BoxGroup("Card")] [SerializeField] private Image icon;
        [BoxGroup("Card")] [SerializeField] private Text itemName;
        [BoxGroup("Card")] [SerializeField] private Text itemDesc;
        [BoxGroup("Card")] [SerializeField] private Text price;
        [BoxGroup("Card")] [SerializeField] private Button buyButton;

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
            price.text = $"Item cost: {info.Price.GetMoney()}";
            
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
            MainMessageBroker.Instance.Publish(newGoodEvent);
            
            Close();
        }

        private void HandleDonatePurchase()
        {
            Close();
        }

        private void BuyItemClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            
            var itemType = StoreItemPurchaser.GetStoreItemType(_info);
            switch (itemType)
            {
                case StoreItemType.Donate:
                    MainMessageBroker.Instance.Publish(new SpendDonateRequest{Amount = _info.Price});
                    break;
                
                case StoreItemType.Game:
                    MainMessageBroker.Instance.Publish(new SpendMoneyRequest{Amount = _info.Price});    
                    break;
                
                case StoreItemType.Purchase:
                    _purchaser.PurchaseStoreItem(_info);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        protected override void BeforePageOpen()
        {
            MainMessageBroker.Instance
                .Receive<SpendDonateResponse>()
                .Subscribe(e => HandleItemPurchase(e.OK))
                .AddTo(_disposable);
            MainMessageBroker.Instance
                .Receive<SpendMoneyResponse>()
                .Subscribe(e => HandleItemPurchase(e.OK))
                .AddTo(_disposable);
            MainMessageBroker.Instance
                .Receive<DonateAddedEvent>()
                .Subscribe(_ => HandleDonatePurchase())
                .AddTo(_disposable);
            MainMessageBroker.Instance
                .Receive<NoAdsPurchaseEvent>()
                .Subscribe(_ => HandleDonatePurchase())
                .AddTo(_disposable);
        }

        protected override void AfterPageClose()
        {
            _disposable.Clear();
        }
    }
}