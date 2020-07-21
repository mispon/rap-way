using Data;
using Enums;
using Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Rappers
{
    /// <summary>
    /// Персональная страница существующего исполнителя
    /// </summary>
    public class RapperPage : Page
    {
        [Header("Поля информации репера")]
        [SerializeField] private Image avatar;
        [SerializeField] private Text nickname;
        [SerializeField] private Text description;
        [SerializeField] private Text fans;
        [SerializeField] private Button battleButton;
        [SerializeField] private Button featButton;

        [Header("Менеджер игрока")]
        [SerializeField] private int cooldown;
        [SerializeField] private Text managerStatusInfo;

        [Header("Страница переговоров")]
        [SerializeField] private RapperWorkingPage workingPage;

        private RapperInfo _rapper;

        private void Start()
        {
            battleButton.onClick.AddListener(() => StartConversation(false));
            featButton.onClick.AddListener(() => StartConversation(true));
        }

        /// <summary>
        /// Обработчик кнопок
        /// </summary>
        private void StartConversation(bool isFeat)
        {
            PlayerManager.SetTeammateCooldown(TeammateType.Manager, cooldown);
            workingPage.StartConversation(_rapper, isFeat);
            Close();
        }
        
        /// <summary>
        /// Открывает персональную страницу репера
        /// </summary>
        public void OpenPage(RapperInfo rapperInfo)
        {
            _rapper = rapperInfo;
            
            DisplayInfo(rapperInfo);
            Open();
        }

        /// <summary>
        /// Отобажает информацию в UI
        /// </summary>
        private void DisplayInfo(RapperInfo info)
        {
            avatar.sprite = info.Avatar;
            nickname.text = info.Name;
            description.text = LocalizationManager.Instance.Get(info.DescKey);
            fans.text = info.Fans.ToString();
        }

        /// <summary>
        /// Вызывается перед открытием страницы
        /// </summary>
        protected override void BeforePageOpen()
        {
            var manager = PlayerManager.Data.Team.Manager;

            string message = "";
            
            if (manager.IsEmpty)
                message = "no_manager";

            if (manager.Cooldown > 0)
                message =  "manager_cooldown";

            SetInteractions(message);
        }

        /// <summary>
        /// Устанавливает состояние возможности взаимодействия с репером
        /// </summary>
        private void SetInteractions(string message)
        {
            var canInteract = string.IsNullOrEmpty(message);
            
            battleButton.interactable = canInteract;
            featButton.interactable = canInteract;
            
            managerStatusInfo.text = canInteract ? "" : LocalizationManager.Instance.Get(message);
        }

        /// <summary>
        /// Вызывается после закрытия страницы
        /// </summary>
        protected override void AfterPageClose()
        {
            _rapper = null;
        }
    }
}