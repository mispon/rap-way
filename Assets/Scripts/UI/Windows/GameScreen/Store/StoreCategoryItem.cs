using Core;
using Game.Player.Inventory.Desc;
using MessageBroker;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using UI.Controls.ScrollViewController;
using UI.Enums;
using UnityEngine;
using UnityEngine.UI;
using StoreItemInfo = ScriptableObjects.StoreItem;

namespace UI.Windows.GameScreen.Store
{
    public class StoreCategoryItem : MonoBehaviour, IScrollViewControllerItem
    {
        [Header("View")]
        [SerializeField] private StoreItemsView itemsView;

        [Header("Category")]
        [SerializeField] private Image categoryIcon;
        [SerializeField] private Text   categoryName;
        [SerializeField] private Button itemsViewButton;

        private RectTransform _rectTransform;

        private int   _index  { get; set; }
        private float _height { get; set; }
        private float _width  { get; set; }

        private InventoryType   _categoryType;
        private StoreItemInfo[] _storeItems;

        private void Start()
        {
            itemsViewButton.onClick.AddListener(() => HandleClick());
        }

        public void HandleClick(bool silent = false)
        {
            if (!silent)
            {
                SoundManager.Instance.PlaySound(UIActionType.Click);
            }

            if (_categoryType == InventoryType.Clothes)
            {
                MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.Shop_Clothes));
            } else
            {
                var category = _index - 1;
                itemsView.Show(category, _storeItems);
            }
        }

        public void Initialize(int i, InventoryType cType, Sprite cIcon, string cName, StoreItemInfo[] storeItems)
        {
            _index        = i;
            _storeItems   = storeItems;
            _categoryType = cType;

            categoryIcon.sprite = cIcon;
            categoryName.text   = cName;
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

            var pos = Vector2.down * (spacing * (_index - 1) + _height * (_index - 1));
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