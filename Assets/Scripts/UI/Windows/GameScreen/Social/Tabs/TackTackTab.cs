using Enums;
using Game.Player;
using Models.Production;
using UI.Windows.GameScreen.Social.Tabs;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Pages.Social.Tabs
{
    /// <summary>
    /// Вкладка псевдо-тиктока
    /// </summary>
    public class TackTackTab : BaseSocialsTab
    {
        [SerializeField] private InputField input;
        [SerializeField] private Text hashtag;
        [Space]
        [SerializeField] private Button challengeButton;
        [SerializeField] private Button danceButton;
        [SerializeField] private Button memeButton;

        private int _modeIndex;
        
        protected override void TabStart()
        {
            challengeButton.onClick.AddListener(() => ChangeMode(0));
            danceButton.onClick.AddListener(() => ChangeMode(1));
            memeButton.onClick.AddListener(() => ChangeMode(2));
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
            challengeButton.interactable = index != 0;
            danceButton.interactable = index != 1;
            memeButton.interactable = index != 2;
        }
        
        /// <summary>
        /// Возвращает информацию соц. действия 
        /// </summary>
        protected override SocialInfo GetInfo()
        {
            return new SocialInfo
            {
                Type = SocialType.TackTack,
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