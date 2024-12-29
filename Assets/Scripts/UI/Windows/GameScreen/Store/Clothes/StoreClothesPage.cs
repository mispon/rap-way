using System;
using CharacterCreator2D;
using Core;
using Core.PropertyAttributes;
using Enums;
using Extensions;
using MessageBroker;
using MessageBroker.Messages.Player.State;
using ScriptableObjects;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Store.Clothes
{
    [Serializable]
    public class CategoryRow
    {
        public SlotCategory Slot;
        public Button       Button;
        public GameObject   Category;
        public GameObject   CategoryColors;
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

        private GameObject _prevCategory;
        private GameObject _prevCategoryColor;

        private readonly CompositeDisposable _disposable = new();

        public override void Initialize()
        {
            foreach (var row in rows)
            {
                row.Button.onClick.AddListener(() => SelectCategory(row));
            }
        }

        protected override void BeforeShow(object ctx = null)
        {
            MsgBroker.Instance
                .Receive<MoneyChangedMessage>()
                .Subscribe(e => UpdateGameBalance(e.NewVal))
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<FullStateResponse>()
                .Subscribe(UpdateHUD)
                .AddTo(_disposable);

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
                _prevCategory.SetActive(false);
            }

            if (_prevCategoryColor != null)
            {
                _prevCategoryColor.SetActive(false);
            }


            row.Category.SetActive(true);
            _prevCategory = row.Category;

            row.CategoryColors.gameObject.SetActive(true);
            _prevCategoryColor = row.CategoryColors;

            foreach (var r in rows)
            {
                r.Button.image.sprite = row.Button.name == r.Button.name
                    ? selectedSprite
                    : mainSprite;
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

        protected override void AfterHide()
        {
            _disposable.Clear();
        }
    }
}