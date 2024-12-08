using CharacterCreator2D;
using Core;
using ScriptableObjects;
using TMPro;
using UI.Controls.ScrollViewController;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.MainMenu.NewGame
{
    [RequireComponent(typeof(Button), typeof(Image), typeof(RectTransform))]
    public class SlotOption : MonoBehaviour, IScrollViewControllerItem
    {
        [SerializeField] private SlotCategory slot;
        [Space]
        [SerializeField] private Color colorEven;
        [SerializeField] private Color colorOdd;

        private RectTransform   _rectTransform;
        private CharacterViewer _character;
        private Part            _part;

        private int   _index  { get; set; }
        private float _height { get; set; }
        private float _width  { get; set; }

        public void Initialize(int pos, CharacterViewer character, Part part)
        {
            _index     = pos;
            _character = character;
            _part      = part;

            GetComponent<Button>().onClick.AddListener(HandleClick);
            GetComponent<Image>().color                    = _index % 2 == 0 ? colorEven : colorOdd;
            GetComponentInChildren<TextMeshProUGUI>().text = _index < 10 ? $"0{_index}" : $"{_index}";

            gameObject.SetActive(true);
        }

        private void HandleClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Switcher);
            _character.EquipPart(slot, _part);
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

            var pos = Vector2.down * (spacing * (_index - 1) + _height * (_index - 1));
            _rectTransform.anchoredPosition = pos;
        }
    }
}