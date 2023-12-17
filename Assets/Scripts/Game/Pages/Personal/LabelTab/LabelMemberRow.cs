using Data;
using Enums;
using Game.UI.ScrollViewController;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Personal.LabelTab
{
    public class LabelMemberRow : MonoBehaviour, IScrollViewControllerItem
    {
        [SerializeField] private Image row;
        [SerializeField] private Image avatar;
        [SerializeField] private Text nickname;
        [SerializeField] private Text fans;
        [SerializeField] private Text score;
        [Space]
        [SerializeField] private Color oddColor;
        [SerializeField] private Color evenColor;
        [Space]
        [SerializeField] private Sprite playerMaleAvatar;
        [SerializeField] private Sprite playerFemaleAvatar;

        private RectTransform _rectTransform;

        private int _index { get; set; }
        private float _height { get; set; }
        
        public void Initialize(int index, RapperInfo info)
        {
            _index = index;

            avatar.sprite = info.IsPlayer 
                ? PlayerManager.Data.Info.Gender == Gender.Male
                    ? playerMaleAvatar
                    : playerFemaleAvatar
                : info.Avatar;
            nickname.text = info.IsPlayer ? $"<color=#00F475>{info.Name}</color>" : info.Name;
            fans.text = $"{info.Fans}M";

            if (info.IsPlayer)
            {
                // hack for properly getting score
                info.Fans *= 1_000_000;
            }

            score.text = RappersManager.GetRapperScore(info).ToString();

            row.color = index % 2 == 0 ? evenColor : oddColor;
        }
        
        public void SetPosition(float spacing)
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
             
            if (_height == 0)
                _height = _rectTransform.rect.height;
            
            var pos = Vector2.down * ((spacing * (_index-1)) + (_height * (_index-1)));
            _rectTransform.anchoredPosition = pos;
        }

        public float GetHeight()
        {
            return _height;
        }
    }
}