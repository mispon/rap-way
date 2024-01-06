using System;
using Data;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Store
{
    public class StoreItemCard : Page
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text price;
        [SerializeField] private Button buyButton;

        private GoodInfo _info;
        
        private void Start()
        {
            buyButton.onClick.AddListener(BuyItem);
        }

        public void Show(GoodInfo info)
        {
            _info = info;
            
            icon.sprite = info.SquareImage;
            price.text = $"PRICE: {info.Price.GetMoney()}".ToUpper();
            
            Open();
        }

        private void BuyItem()
        {
            
        }
    }
}