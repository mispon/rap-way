using CharacterCreator2D;
using Core;
using ScriptableObjects;
using UI.Controls.ScrollViewController;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.MainMenu.NewGame
{
    [RequireComponent(typeof(Button), typeof(Image), typeof(RectTransform))]
    public class SlotColorOption : MonoBehaviour, IScrollViewControllerItem
    {
        [SerializeField] private SlotCategory slot;

        private RectTransform   _rectTransform;
        private CharacterViewer _character;
        private Part            _part;

        private int   _index  { get; set; }
        private float _height { get; set; }
        private float _width  { get; set; }

        public void Initialize(int pos, CharacterViewer character, Color color)
        {
            _index     = pos;
            _character = character;

            GetComponent<Button>().onClick.AddListener(HandleClick);
            GetComponent<Image>().color = color;

            gameObject.SetActive(true);
        }

        public void SetPartColor(Color color)
        {
            if (slot == SlotCategory.BodySkin)
            {
                _character.SkinColor = color;
            } else
            {
                _character.SetPartColor(slot, color, color, color);
            }
        }

        private void HandleClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Switcher);

            var color = GetComponent<Image>().color;
            SetPartColor(color);
        }

        public float GetHeight()
        {
            return _height;
        }

        public float GetWidth()
        {
            return _width;
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

            var pos = Vector2.right * (spacing * (_index - 1) + _height * (_index - 1));
            _rectTransform.anchoredPosition = pos;
        }
    }
}