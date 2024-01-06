using Core;
using Data;
using Messages.State;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Store
{
    public class StoreItemCard : Page
    {
        private readonly CompositeDisposable _disposable = new();
        
        [SerializeField] private Image icon;
        [SerializeField] private Text price;
        [SerializeField] private Button buyButton;

        private GoodInfo _info;
        
        private void Start()
        {
            buyButton.onClick.AddListener(BuyItemClick);
        }

        public void Show(GoodInfo info)
        {
            _info = info;
            
            icon.sprite = info.SquareImage;
            price.text = $"PRICE: {info.Price.GetMoney()}".ToUpper();
            
            Open();
        }

        private void HandleItemPurchase(SpendMoneyResponse resp)
        {
            if (!resp.OK)
            {
                // show error
                Debug.Log("Not enough money");
                return;
            }
            
            SoundManager.Instance.PlaySound(UIActionType.Pay);
            Debug.Log("Store item purchased!");
            
            // todo: add store item to player
            
            Close();
        }

        private void BuyItemClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            SendMessage(new SpendMoneyRequest{Amount = _info.Price});
        }
        
        protected override void BeforePageOpen()
        {
            RecvMessage<SpendMoneyResponse>(HandleItemPurchase, _disposable);
        }

        protected override void AfterPageClose()
        {
            _disposable.Clear();
        }
    }
}