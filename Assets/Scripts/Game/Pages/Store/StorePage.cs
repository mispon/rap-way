using System.Collections.Generic;
using Core;
using Data;
using Firebase.Analytics;
using Game.UI.ScrollViewController;
using MessageBroker.Messages.State;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Store
{
    public class StorePage: Page
    {
        private readonly CompositeDisposable _disposable = new();
        
        [BoxGroup("Data")] [SerializeField] private GoodsData data;
        
        [BoxGroup("Header")] [SerializeField] private Text gameBalance;
        [BoxGroup("Header")] [SerializeField] private Text donateBalance;
        
        [BoxGroup("Categories")] [SerializeField] private ScrollViewController categories;
        [BoxGroup("Categories")] [SerializeField] private GameObject categoryItemTemplate;

        private readonly List<StoreCategoryItem> _categoryItems = new();
        
        protected override void BeforePageOpen()
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.ShopOpened);

            RecvMessage<MoneyChangedEvent>(e => UpdateGameBalance(e.NewVal), _disposable);
            RecvMessage<DonateChangedEvent>(e => UpdateDonateBalance(e.NewVal), _disposable);
            RecvMessage<FullStateResponse>(UpdateHUD, _disposable);
            SendMessage(new FullStateRequest());
            
            ShowCategoriesList();
        }

        protected override void AfterPageOpen()
        {
            _categoryItems[0].ShowItems(true);
        }

        private void ShowCategoriesList()
        {
            int i = 1;
            foreach (var (type, itemsInfo) in data.Items)
            {
                var row = categories.InstantiatedElement<StoreCategoryItem>(categoryItemTemplate);

                var categoryIcon = data.Categories[type];
                row.Initialize(i, categoryIcon, GetLocale(type.GetDescription()), itemsInfo);
                i++;
                
                _categoryItems.Add(row);
            }
            
            categories.RepositionElements(_categoryItems);
        }

        private void UpdateHUD(FullStateResponse resp)
        {
            UpdateGameBalance(resp.Money);
            UpdateDonateBalance(resp.Donate);
        }
        
        private void UpdateGameBalance(int money)
        {
            gameBalance.text = money.GetMoney();
        }
        
        private void UpdateDonateBalance(int donate)
        {
            donateBalance.text = donate.GetMoney();
        }

        protected override void AfterPageClose()
        {
            foreach (var item in _categoryItems)
            {
                Destroy(item.gameObject);
            }
            
            _categoryItems.Clear();
            _disposable.Clear();
        }
    }
}