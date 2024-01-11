using System;
using Core;
using Data;
using Enums;
using Game.Pages.Store.Purchase;
using Game.UI.ScrollViewController;
using MessageBroker.Messages.Donate;
using MessageBroker.Messages.Goods;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Store
{
    public class StoreItem : MonoBehaviour, IScrollViewControllerItem
    {
        private IDisposable _singleDispose;
        private readonly CompositeDisposable _disposable = new();
        
        [BoxGroup("Card")] [SerializeField] private StoreItemPurchaseCard itemCard;
        [Space]
        [BoxGroup("Item")] [SerializeField] private Image icon;
        [BoxGroup("Item")] [SerializeField] private Text price;
        [BoxGroup("Item")] [SerializeField] private Button itemButton;
        
        private RectTransform _rectTransform;
        
        private int _index { get; set; }
        private float _height { get; set; }
        private float _width { get; set; }
        
        private GoodInfo _info;
        
        private void Start()
        {
            itemButton.onClick.AddListener(ShowItemCard);
        }

        private void ShowItemCard()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            itemCard.Show(_info);
        }

        public void Initialize(int i, GoodInfo info)
        {
            _index = i;
            _info = info;
            
            icon.sprite = info.SquareImage;
            price.text = info.Price.GetMoney();

            HandleEvents();
        }

        private void HandleEvents()
        {
            var messageBroker = GameManager.Instance.MessageBroker;
            
            messageBroker
                .Receive<AddNewGoodEvent>()
                .Subscribe(e => OnItemPurchased(e.Type, e.Level))
                .AddTo(_disposable);
            messageBroker
                .Receive<NoAdsPurchaseEvent>()
                .Subscribe(_ => OnNoAdsPurchased())
                .AddTo(_disposable);
            
            _singleDispose = messageBroker
                .Receive<GoodExistsResponse>()
                .Subscribe(e =>
                {
                    if (e.Status) 
                        SetPurchased();
                    
                    _singleDispose?.Dispose();
                });

            messageBroker.Publish(_info is NoAds
                ? new GoodExistsRequest {IsNoAds = true}
                : new GoodExistsRequest {Type = _info.Type, Level = _info.Level});
        }

        private void OnItemPurchased(GoodsType type, int level)
        {
            if (level == _info.Level && type == _info.Type)
            {
                SetPurchased();
            }
        }

        private void OnNoAdsPurchased()
        {
            if (_info is NoAds)
            {
                SetPurchased();
            }
        }

        private void SetPurchased()
        {
            itemButton.interactable = false;
            itemButton.image.color = Color.cyan;
        }
        
        public void SetPosition(float spacing)
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
             
            if (_height == 0)
                _height = _rectTransform.rect.height;
            if (_width == 0)
                _width = _rectTransform.rect.width;
            
            var pos = Vector2.right * ((spacing * (_index-1)) + (_width * (_index-1)));
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