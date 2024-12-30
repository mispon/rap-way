using System;
using CharacterCreator2D;
using Core;
using Game.Player.Character;
using MessageBroker;
using MessageBroker.Messages.Store;
using ScriptableObjects;
using UI.Controls.ScrollViewController;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterCreator
{
    [RequireComponent(typeof(Button), typeof(RectTransform))]
    public class SlotColorOption : MonoBehaviour, IScrollViewControllerItem
    {
        [SerializeField] private SlotCategory slot;
        [SerializeField] private GameObject   outline;

        private RectTransform _rectTransform;
        private Part          _part;

        private int   _index  { get; set; }
        private float _height { get; set; }
        private float _width  { get; set; }

        public void Initialize(int pos, SlotColor sc, Action beforeClick)
        {
            _index = pos;

            GetComponent<Button>().onClick.AddListener(() =>
            {
                beforeClick.Invoke();
                HandleClick();
            });

            var images = GetComponentsInChildren<Image>();
            images[0].color = sc.Color1;
            images[1].color = sc.Color2;
            images[2].color = sc.Color3;

            gameObject.SetActive(true);
            outline.SetActive(false);
        }

        public void SelectOption()
        {
            var images = GetComponentsInChildren<Image>();

            var color1 = images[0].color;
            var color2 = images[1].color;
            var color3 = images[2].color;

            if (slot == SlotCategory.BodySkin)
            {
                Character.Instance.Viewer.SkinColor = color1;
            } else
            {
                Character.Instance.Viewer.SetPartColor(slot, color1, color2, color3);
                MsgBroker.Instance.Publish(new ClothesSlotColorChangedMessage {Slot = slot});
            }

            outline.SetActive(true);
        }

        public void HideOutline()
        {
            outline.SetActive(false);
        }

        private void HandleClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Switcher);
            SelectOption();
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