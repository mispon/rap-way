using System;
using CharacterCreator2D;
using Core;
using Core.Localization;
using Game.Player.Character;
using ScriptableObjects;
using TMPro;
using UI.Controls.ScrollViewController;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterCreator
{
    [RequireComponent(typeof(Button), typeof(Image), typeof(RectTransform))]
    public class SlotOption : MonoBehaviour, IScrollViewControllerItem
    {
        [SerializeField] private SlotCategory slot;
        [SerializeField] private GameObject   outline;
        [Space]
        [SerializeField] private Image image;
        [SerializeField] private Color colorEven;
        [SerializeField] private Color colorOdd;

        private RectTransform _rectTransform;
        private Part          _part;

        private int   _index  { get; set; }
        private float _height { get; set; }
        private float _width  { get; set; }

        public void Initialize(int pos, Part part, Action beforeClick)
        {
            _index = pos;
            _part  = part;

            GetComponent<Button>().onClick.AddListener(() =>
            {
                beforeClick.Invoke();
                HandleClick();
            });
            GetComponentInChildren<TextMeshProUGUI>().text = _index == 0 && slot is SlotCategory.Hair or SlotCategory.FacialHair
                ? LocalizationManager.Instance.Get("empty")
                : _index < 10
                    ? $"0{_index}"
                    : $"{_index}";

            image.color = _index % 2 == 0 ? colorEven : colorOdd;

            gameObject.SetActive(true);
            outline.SetActive(false);
        }

        public void SetPart(Part part)
        {
            Character.Instance.Viewer.EquipPart(slot, part);
        }

        public void HideOutline()
        {
            outline.SetActive(false);
        }

        private void HandleClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Switcher);
            SetPart(_part);
            outline.SetActive(true);
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