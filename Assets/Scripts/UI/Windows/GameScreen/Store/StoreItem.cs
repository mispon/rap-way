using System;
using System.Collections.Generic;
using Core;
using Extensions;
using Game.Player.Inventory.Desc;
using MessageBroker;
using MessageBroker.Messages.Player;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using UI.Controls.ScrollViewController;
using UI.Enums;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using StoreItemInfo = ScriptableObjects.StoreItem;

namespace UI.Windows.GameScreen.Store
{
    public class StoreItem : MonoBehaviour, IScrollViewControllerItem
    {
        private          IDisposable         _singleDispose;
        private readonly CompositeDisposable _disposable = new();

        [Header("Money Icon")]
        [SerializeField] private Sprite moneySprite;
        [SerializeField] private Sprite donateSprite;
        [SerializeField] private Sprite realMoneySprite;

        [Space] [Header("Item")]
        [SerializeField] private Image icon;
        [SerializeField] private Text       price;
        [SerializeField] private Image      priceIcon;
        [SerializeField] private Button     itemButton;
        [SerializeField] private GameObject purchased;

        private RectTransform _rectTransform;

        private int   _index  { get; set; }
        private float _height { get; set; }
        private float _width  { get; set; }

        private int           _category;
        private StoreItemInfo _info;

        private void Start()
        {
            itemButton.onClick.AddListener(ShowItemCard);
        }

        private void ShowItemCard()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type = WindowType.Shop_ItemCard,
                Context = new Dictionary<string, object>
                {
                    ["item_info"] = _info,
                    ["category"]  = _category
                }
            });
        }

        public void Initialize(int idx, int category, StoreItemInfo info)
        {
            _index    = idx;
            _category = category;
            _info     = info;

            icon.sprite      = info.SquareImage;
            price.text       = info.Price.GetDisplay();
            priceIcon.sprite = GetMoneyIcon(info);

            HandleEvents();
        }

        private Sprite GetMoneyIcon(StoreItemInfo item)
        {
            return item.Type switch
            {
                InventoryType.DonateCoins => realMoneySprite,
                InventoryType.NoAds       => realMoneySprite,
                _                         => item.IsDonate ? donateSprite : moneySprite
            };
        }

        private void HandleEvents()
        {
            MsgBroker.Instance
                .Receive<AddInventoryItemMessage>()
                .Subscribe(e => OnItemPurchased(e.Type, e.Name))
                .AddTo(_disposable);

            _singleDispose = MsgBroker.Instance
                .Receive<InventoryItemExistsResponse>()
                .Subscribe(e =>
                {
                    if (e.Status)
                    {
                        SetPurchased();
                    }

                    _singleDispose?.Dispose();
                });

            MsgBroker.Instance.Publish(_info.Type == InventoryType.NoAds
                ? new InventoryItemExistsRequest {IsNoAds = true}
                : new InventoryItemExistsRequest
                {
                    Name = _info.Name,
                    Type = _info.Type
                });
        }

        private void OnItemPurchased(InventoryType type, string itemName)
        {
            if (itemName == _info.Name && type == _info.Type)
            {
                SetPurchased();
            }
        }

        private void SetPurchased()
        {
            itemButton.gameObject.SetActive(false);
            purchased.SetActive(true);
        }

        public void SetPosition(float spacing)
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            if (_height == 0)
            {
                _height = _rectTransform.rect.height;
            }

            if (_width == 0)
            {
                _width = _rectTransform.rect.width;
            }

            var pos = Vector2.right * (spacing * (_index - 1) + _width * (_index - 1));
            _rectTransform.anchoredPosition = pos;
        }

        public float GetHeight()
        {
            return _height;
        }

        public float GetWidth()
        {
            return _width;
        }

        private void OnDestroy()
        {
            _disposable.Clear();
        }
    }
}