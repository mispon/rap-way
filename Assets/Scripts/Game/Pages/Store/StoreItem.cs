using System;
using Core;
using Data;
using Game.UI.ScrollViewController;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Store
{
    public class StoreItem : MonoBehaviour, IScrollViewControllerItem
    {
        [SerializeField] private Image icon;
        [SerializeField] private StoreItemCard itemCard;
        [SerializeField] private Button itemButton;
        
        private RectTransform _rectTransform;
        
        private int _index { get; set; }
        private float _height { get; set; }
        private float _width { get; set; }

        private GoodUI _info;
        
        private void Start()
        {
            itemButton.onClick.AddListener(ShowItemCard);
        }

        private void ShowItemCard()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            itemCard.Show(_info);
        }

        public void Initialize(int i, GoodUI info)
        {
            _index = i;
            _info = info;

            icon.sprite = info.SquareImage;
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