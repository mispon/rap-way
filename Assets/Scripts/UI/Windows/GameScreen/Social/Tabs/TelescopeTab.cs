using Enums;
using Game.Player;
using Models.Production;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Pages.Social.Tabs
{
    /// <summary>
    /// Вкладка псевдо-перескопа
    /// </summary>
    public class TelescopeTab : BaseSocialsTab
    {
        [SerializeField] private InputField input;
        [SerializeField] private Text hashtag;
        [Space]
        [SerializeField] private Button rappingButton;
        [SerializeField] private Button talkingButton;
        [SerializeField] private Button exploreButton;

        private int _modeIndex;
        
        protected override void TabStart()
        {
            rappingButton.onClick.AddListener(() => ChangeMode(0));
            talkingButton.onClick.AddListener(() => ChangeMode(1));
            exploreButton.onClick.AddListener(() => ChangeMode(2));
        }

        /// <summary>
        /// Обработчик изменения мода 
        /// </summary>
        private void ChangeMode(int index)
        {
            _modeIndex = index;
            SetButtonSelected(index);
        }

        /// <summary>
        /// Устанавливает цвет выделенной кнопки 
        /// </summary>
        private void SetButtonSelected(int index)
        {
            rappingButton.interactable = index != 0;
            talkingButton.interactable = index != 1;
            exploreButton.interactable = index != 2;
        }

        /// <summary>
        /// Возвращает информацию соц. действия 
        /// </summary>
        protected override SocialInfo GetInfo()
        {
            return new SocialInfo
            {
                Type = SocialType.Telescope,
                MainText = input.text,
                ModeIndex = _modeIndex
            };
        }
        
        /// <summary>
        /// Вызывается при открытии вкладки
        /// </summary>
        protected override void OnOpen()
        {
            base.OnOpen();
            ChangeMode(0);
            hashtag.text = $"#{PlayerManager.Data.Info.NickName}";
            input.text = string.Empty;
        }
    }
}