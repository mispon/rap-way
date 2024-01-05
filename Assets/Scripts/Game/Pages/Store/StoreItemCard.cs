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

        private GoodUI _info;
        
        private void Start()
        {
            buyButton.onClick.AddListener(BuyItem);
        }

        public void Show(GoodUI info)
        {
            _info = info;
            
            icon.sprite = info.Image;
            price.text = $"PRICE: {info.Price.GetMoney()}".ToUpper();
            
            Open();
        }

        private void BuyItem()
        {
            
        }
    }
}