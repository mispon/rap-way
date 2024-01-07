using Core;
using Data;
using Game.UI.GameError;
using MessageBroker.Messages.State;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Store.Purchase
{
    public class StoreItemPurchaseCard : Page
    {
        private readonly CompositeDisposable _disposable = new();

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
            SendMessage(newGoodEvent);
            
            Close();
        }

        private void BuyItemClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);

            if (StoreItemPurchaser.IsDonateCoins(_info))
            {
                // todo:
                return;
            }
            
            if (StoreItemPurchaser.IsDonateItem(_info))
                SendMessage(new SpendDonateRequest{Amount = _info.Price});
            else 
                SendMessage(new SpendMoneyRequest{Amount = _info.Price});
        }
        
        protected override void BeforePageOpen()
        {
            RecvMessage<SpendMoneyResponse>(e => HandleItemPurchase(e.OK), _disposable);
            RecvMessage<SpendDonateResponse>(e => HandleItemPurchase(e.OK), _disposable);
        }

        protected override void AfterPageClose()
        {
            _disposable.Clear();
        }
    }
}