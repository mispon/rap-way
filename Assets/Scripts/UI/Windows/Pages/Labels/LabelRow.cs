using Core;
using ScriptableObjects;
using UI.Controls.ScrollViewController;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Pages.Labels
{
    public class LabelRow : MonoBehaviour, IScrollViewControllerItem
    {
        [SerializeField] private Image row;
        [SerializeField] private Button rowButton;
        
        [Space]
        [SerializeField] private Text position;
        [SerializeField] private Text labelName;
        [SerializeField] private Text score;
        
        [Space]
        [SerializeField] private Color oddColor;
        [SerializeField] private Color evenColor;

        [Space]
        [SerializeField] private LabelCard card;

        private LabelInfo _labelInfo;
        
        private RectTransform _rectTransform;

        private int _index { get; set; }
        private float _height { get; set; }
        private float _width { get; set; }
        
        private void Start()
        {
            rowButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(UIActionType.Click);
                ShowRapperInfo();
            });
        }

        public void Initialize(int pos, LabelInfo info)
        {
            _labelInfo = info;
            
            _index = pos;
            position.text = $"{pos}.";
            labelName.text = info.IsPlayer ? $"<color=#00F475>{info.Name}</color>" : info.Name;
            score.text = info.Score.ToString();
            
            row.color = pos % 2 == 0 ? evenColor : oddColor;

            if (pos == 1)
            {
                ShowRapperInfo();
            }
        }

        private void ShowRapperInfo()
        {
            card.Show(_labelInfo);
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