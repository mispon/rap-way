using System;
using Core;
using Data;
using Game.UI.ScrollViewController;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Store
{
    public class StoreCategoryItem : MonoBehaviour, IScrollViewControllerItem
    {
        [SerializeField] private StoreItemsView itemsView;
        [SerializeField] private Button itemsViewButton;
        
        private RectTransform _rectTransform;
        
        private int _index { get; set; }
        private float _height { get; set; }
        private float _width { get; set; }

        private GoodInfo _info;

        private void Start()
        {
            itemsViewButton.onClick.AddListener(ShowItems);
        }

        public void ShowItems()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            itemsView.Show(_info);
        }

        public void Initialize(int i, GoodInfo info)
        {
            _index = i;
            _info = info;
        }
        
        public void SetPosition(float spacing)
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
             
            if (_height == 0)
                _height = _rectTransform.rect.height;
            if (_width == 0)
                _width = _rectTransform.rect.width;
            
            var pos = Vector2.down * ((spacing * (_index-1)) + (_height * (_index-1)));
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