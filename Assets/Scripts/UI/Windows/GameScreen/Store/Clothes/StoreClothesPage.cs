using Core;
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
    public class StoreClothesPage : Page
    {
        [Header("Data")]
        [SerializeField] private GoodsData data;

        [Header("Header")]
        [SerializeField] private Text gameBalance;
        [SerializeField] private Text donateBalance;

        [Header("Category Buttons")]
        [SerializeField] private Button hatsButton;
        [SerializeField] private Button outwearButton;
        [SerializeField] private Button pantsButton;
        [SerializeField] private Button skirtsButton;
        [SerializeField] private Button glovesButton;
        [SerializeField] private Button bootsButton;
        [SerializeField] private Button otherButton;

        [Header("Categories")]
        [SerializeField] private GameObject hats;
        [SerializeField] private GameObject outwear;
        [SerializeField] private GameObject pants;
        [SerializeField] private GameObject skirts;
        [SerializeField] private GameObject gloves;
        [SerializeField] private GameObject boots;
        [SerializeField] private GameObject other;

        [Header("Categories Colors")]
        [SerializeField] private GameObject hatsColor;
        [SerializeField] private GameObject outwearColor;
        [SerializeField] private GameObject pantsColor;
        [SerializeField] private GameObject skirtsColor;
        [SerializeField] private GameObject glovesColor;
        [SerializeField] private GameObject bootsColor;
        [SerializeField] private GameObject otherColor;

        private GameObject _prevCategory;
        private GameObject _prevCategoryColor;

        private readonly CompositeDisposable _disposable = new();

        public override void Initialize()
        {
            hatsButton.onClick.AddListener(() => SelectCategory(hats, hatsColor));
            outwearButton.onClick.AddListener(() => SelectCategory(outwear, outwearColor));
            pantsButton.onClick.AddListener(() => SelectCategory(pants, pantsColor));
            skirtsButton.onClick.AddListener(() => SelectCategory(skirts, skirtsColor));
            glovesButton.onClick.AddListener(() => SelectCategory(gloves, glovesColor));
            bootsButton.onClick.AddListener(() => SelectCategory(boots, bootsColor));
            otherButton.onClick.AddListener(() => SelectCategory(other, otherColor));
            
            // handle clothes slot change
            // handle clothes slot color change
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
            SelectCategory(hats, hatsColor);

            if (PlayerAPI.Data.Info.Gender == Gender.Male)
            {
                skirtsButton.gameObject.SetActive(false);
            }
        }

        private void SelectCategory(GameObject category, GameObject categoryColors)
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


            category.SetActive(true);
            _prevCategory = category;

            categoryColors.gameObject.SetActive(true);
            _prevCategoryColor = categoryColors;
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