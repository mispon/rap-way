using System;
using System.Linq;
using CharacterCreator2D;
using Core;
using Core.PropertyAttributes;
using Enums;
using Extensions;
using MessageBroker;
using MessageBroker.Messages.Player.State;
using MessageBroker.Messages.Store;
using ScriptableObjects;
using UI.CharacterCreator;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Store.Clothes
{
    [Serializable]
    public class CategoryRow
    {
        public SlotCategory            Slot;
        public Button                  Button;
        public StoreClothesOptionsList Category;
        public SlotColorOptionsList    CategoryColors;
    }

    public class StoreClothesPage : Page
    {
        [Header("Header")]
        [SerializeField] private Text gameBalance;
        [SerializeField] private Text donateBalance;

        [Header("Categories")]
        [ArrayElementTitle(new[] {"Slot"})]
        [SerializeField] private CategoryRow[] rows;

        [Header("Button Images")]
        [SerializeField] private Sprite mainSprite;
        [SerializeField] private Sprite selectedSprite;

        private StoreClothesOptionsList _prevCategory;
        private SlotColorOptionsList    _prevCategoryColor;

        private readonly CompositeDisposable _disposable = new();

        public override void Initialize()
        {
            MsgBroker.Instance
                .Receive<MoneyChangedMessage>()
                .Subscribe(e => UpdateGameBalance(e.NewVal))
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<FullStateResponse>()
                .Subscribe(UpdateHUD)
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<ClothesSlotChangedMessage>()
                .Subscribe(HandleClothingItemSelection)
                .AddTo(_disposable);

            foreach (var row in rows)
            {
                row.Button.onClick.AddListener(() => SelectCategory(row));
            }
        }

        protected override void BeforeShow(object ctx = null)
        {
            MsgBroker.Instance.Publish(new FullStateRequest());
            SelectCategory(rows[0]);

            if (PlayerAPI.Data.Info.Gender == Gender.Male)
            {
                const int skirtsIdx = 3;
                rows[skirtsIdx].Button.gameObject.SetActive(false);
            }
        }

        private void SelectCategory(CategoryRow row)
        {
            SoundManager.Instance.PlaySound(UIActionType.Switcher);

            if (_prevCategory != null)
            {
                _prevCategory.Hide();
            }

            if (_prevCategoryColor != null)
            {
                _prevCategoryColor.Hide();
            }

            row.Category.Show();

            _prevCategory      = row.Category;
            _prevCategoryColor = row.CategoryColors;

            foreach (var r in rows)
            {
                r.Button.image.sprite = row.Button.name == r.Button.name
                    ? selectedSprite
                    : mainSprite;
            }
        }

        private void HandleClothingItemSelection(ClothesSlotChangedMessage m)
        {
            var category = rows.First(e => e.Slot == m.Slot);

            if (m.Index == 0)
            {
                category.CategoryColors.Hide();
            } else
            {
                category.CategoryColors.Show();
            }
        }

        private void UpdateHUD(FullStateResponse resp)
        {
            UpdateGameBalance(resp.Money);
            UpdateDonateBalance(resp.Donate);
        }

        private void UpdateGameBalance(int money)
        {
            gameBalance.text = money.GetShort();
        }

        private void UpdateDonateBalance(int donate)
        {
            donateBalance.text = donate.GetDisplay();
        }

        private void OnDestroy()
        {
            _disposable.Clear();
        }
    }
}