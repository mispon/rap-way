using Enums;
using Models.Info;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Social.Tabs
{
    /// <summary>
    /// Вкладка псевдо-инстаграма
    /// </summary>
    public class IeyegramTab : BaseSocialsTab
    {
        [SerializeField] private InputField input;
        [SerializeField] private Text hashtag;
        [Space]
        [SerializeField] private Button lifeButton;
        [SerializeField] private Button foodButton;
        [SerializeField] private Button musicButton;

        private int _modeIndex;
        
        protected override void TabStart()
        {
            lifeButton.onClick.AddListener(() => ChangeMode(0));
            foodButton.onClick.AddListener(() => ChangeMode(1));
            musicButton.onClick.AddListener(() => ChangeMode(2));
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
            lifeButton.interactable = index != 0;
            foodButton.interactable = index != 1;
            musicButton.interactable = index != 2;
        }

        /// <summary>
        /// Возвращает информацию соц. действия 
        /// </summary>
        protected override SocialInfo GetInfo()
        {
            return new SocialInfo
            {
                Type = SocialType.Ieyegram,
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