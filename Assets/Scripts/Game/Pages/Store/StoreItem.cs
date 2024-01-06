using Core;
using Data;
using Game.UI.ScrollViewController;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Store
{
    public class StoreItem : MonoBehaviour, IScrollViewControllerItem
    {
        [BoxGroup("Card")] [SerializeField] private StoreItemCard itemCard;
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
    }
}