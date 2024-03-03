using System.Collections.Generic;
using Enums;
using Extensions;
using Firebase.Analytics;
using MessageBroker;
using MessageBroker.Messages.Player;
using MessageBroker.Messages.Player.State;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UI.Controls.ScrollViewController;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Store
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
        
        protected override void BeforeShow()
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.ShopOpened);

            MsgBroker.Instance
                .Receive<MoneyChangedMessage>()
                .Subscribe(e => UpdateGameBalance(e.NewVal))
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<DonateChangedMessage>()
                .Subscribe(e => UpdateDonateBalance(e.NewVal))
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<FullStateResponse>()
                .Subscribe(UpdateHUD)
                .AddTo(_disposable);
            MsgBroker.Instance.Publish(new FullStateRequest());
            
            ShowCategoriesList();
        }

        protected override void AfterShow()
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
            gameBalance.text = money.GetDisplay();
        }
        
        private void UpdateDonateBalance(int donate)
        {
            donateBalance.text = donate.GetDisplay();
        }

        protected override void AfterHide()
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