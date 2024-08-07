using System.Collections.Generic;
using System.Linq;
using Core.Context;
using Enums;
using Extensions;
using Firebase.Analytics;
using MessageBroker;
using MessageBroker.Messages.Player;
using MessageBroker.Messages.Player.State;
using ScriptableObjects;
using UI.Controls.ScrollViewController;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Store
{
    public class StorePage : Page
    {
        private readonly CompositeDisposable _disposable = new();

        [Header("Data")]
        [SerializeField] private GoodsData data;

        [Header("Header")]
        [SerializeField] private Text gameBalance;
        [SerializeField] private Text donateBalance;

        [Header("Categories")]
        [SerializeField] private ScrollViewController categories;
        [SerializeField] private GameObject categoryItemTemplate;

        private readonly List<StoreCategoryItem> _categoryItems = new();

        protected override void BeforeShow(object ctx = null)
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.ShopOpened);

            MsgBroker.Instance
                .Receive<MoneyChangedMessage>()
                .Subscribe(e => UpdateGameBalance(e.NewVal))
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<FullStateResponse>()
                .Subscribe(UpdateHUD)
                .AddTo(_disposable);
            MsgBroker.Instance.Publish(new FullStateRequest());

            ShowCategoriesList();
        }

        protected override void AfterShow(object ctx = null)
        {
            int idx = ctx.Value<int>();
            _categoryItems[idx].ShowItems(true);
        }

        private void ShowCategoriesList()
        {
            int i = 1;
            foreach (var category in data.Categories)
            {
                var row = categories.InstantiatedElement<StoreCategoryItem>(categoryItemTemplate);

                var group = data.Goods.First(e => e.Type == category.Type);
                row.Initialize(i, category.Icon, GetLocale(category.Type.GetDescription()), group.Items);
                i++;

                _categoryItems.Add(row);
            }

            categories.RepositionElements(_categoryItems);
        }

        private void UpdateHUD(FullStateResponse resp)
        {
            UpdateGameBalance(resp.Money);
        }

        private void UpdateGameBalance(int money)
        {
            gameBalance.text = money.GetDisplay();
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