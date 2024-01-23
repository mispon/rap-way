using System.Collections.Generic;
using Enums;
using Extensions;
using Firebase.Analytics;
using MessageBroker.Messages.Donate;
using MessageBroker.Messages.State;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UI.Controls.ScrollViewController;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Pages.Store
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
            foreach (var category in data.Categories)
            {
                var row = categories.InstantiatedElement<StoreCategoryItem>(categoryItemTemplate);

                var itemsInfo = data.Items[category.Type];
                row.Initialize(i, category.Icon, GetLocale(category.Type.GetDescription()), itemsInfo);
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