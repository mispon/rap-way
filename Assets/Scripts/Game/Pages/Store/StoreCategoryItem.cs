using Core;
using Data;
using Game.UI.ScrollViewController;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Store
{
    public class StoreCategoryItem : MonoBehaviour, IScrollViewControllerItem
    {
        [BoxGroup("View")] [SerializeField] private StoreItemsView itemsView;
        
        [BoxGroup("Category")] [SerializeField] private Image categoryIcon;
        [BoxGroup("Category")] [SerializeField] private Text categoryName;
        [BoxGroup("Category")] [SerializeField] private Button itemsViewButton;
        
        private RectTransform _rectTransform;
        
        private int _index { get; set; }
        private float _height { get; set; }
        private float _width { get; set; }

        private GoodInfo[] _itemsInfo;

        private void Start()
        {
            itemsViewButton.onClick.AddListener(ShowItems);
        }

        public void ShowItems()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            itemsView.Show(_itemsInfo);
        }

        public void Initialize(int i, Sprite cIcon, string cName, GoodInfo[] itemsInfo)
        {
            _index = i;
            _itemsInfo = itemsInfo;

            categoryIcon.sprite = cIcon;
            categoryName.text = cName;
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