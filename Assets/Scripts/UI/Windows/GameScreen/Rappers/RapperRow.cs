﻿using Core;
using Extensions;
using Game.Rappers.Desc;
using ScriptableObjects;
using UI.Controls.ScrollViewController;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Rappers
{
    public class RapperRow : MonoBehaviour, IScrollViewControllerItem
    {
        [SerializeField] private Image  row;
        [SerializeField] private Button rowButton;

        [Space]
        [SerializeField] private Text position;
        [SerializeField] private Text nickname;
        [SerializeField] private Text fans;
        [SerializeField] private Text label;

        [Space]
        [SerializeField] private Color oddColor;
        [SerializeField] private Color evenColor;

        [Space]
        [SerializeField] private RapperCard card;

        private RapperInfo    _rapperInfo;
        private RectTransform _rectTransform;

        private int   _index  { get; set; }
        private float _height { get; set; }
        private float _width  { get; set; }

        private void Start()
        {
            rowButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(UIActionType.Click);
                ShowRapperInfo();
            });
        }

        public void Initialize(int pos, RapperInfo info)
        {
            _index        = pos;
            position.text = $"{pos}.";
            nickname.text = info.IsPlayer ? $"<color=#00F475>{info.Name}</color>" : info.Name;
            fans.text     = info.Fans.GetShort();
            label.text    = info.Label != "" ? info.Label : "-";

            row.color = pos % 2 == 0 ? evenColor : oddColor;

            _rapperInfo = info;

            if (pos == 1)
            {
                ShowRapperInfo();
            }
        }

        private void ShowRapperInfo()
        {
            card.Show(_rapperInfo);
        }

        /// <summary>
        ///     Позиционирование элемента в таблице в соответсвии с его порядковым номером
        /// </summary>
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