using Core;
using ScriptableObjects;
using UI.Controls.ScrollViewController;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Store
{
    public class StoreCategoryItem : MonoBehaviour, IScrollViewControllerItem
    {
        [Header("View")]
        [SerializeField] private StoreItemsView itemsView;

        [Header("Category")]
        [SerializeField] private Image categoryIcon;
        [SerializeField] private Text categoryName;
        [SerializeField] private Button itemsViewButton;

        private RectTransform _rectTransform;

        private int _index { get; set; }
        private float _height { get; set; }
        private float _width { get; set; }

        private GoodInfo[] _itemsInfo;

        private void Start()
        {
            itemsViewButton.onClick.AddListener(() => ShowItems());
        }

        public void ShowItems(bool silent = false)
        {
            if (!silent)
            {
                SoundManager.Instance.PlaySound(UIActionType.Click);
            }

            int category = _index - 1;
            itemsView.Show(category, _itemsInfo);
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

            var pos = Vector2.down * ((spacing * (_index - 1)) + (_height * (_index - 1)));
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