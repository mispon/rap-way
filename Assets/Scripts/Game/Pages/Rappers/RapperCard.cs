using Core;
using Data;
using Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Rappers
{
    /// <summary>
    /// Персональная карточка существующего исполнителя
    /// </summary>
    public class RapperCard : MonoBehaviour
    {
        [Header("Поля информации репера")]
        [SerializeField] private Sprite customImage;
        [SerializeField] private Image avatar;
        [SerializeField] private Text nickname;
        [Space]
        [SerializeField] private Text vocobulary;
        [SerializeField] private Text bitmaking;
        [SerializeField] private Text management;
        [Space]
        [SerializeField] private Image managerAvatar;
        [SerializeField] private Button battleButton;
        [SerializeField] private Button featButton;
        [Space]
        [SerializeField] private Text fans;

        [Header("Страницы")]
        [SerializeField] private RappersPage rappersPage;
        [SerializeField] private RapperWorkingPage workingPage;

        [Header("Банк картинок")]
        [SerializeField] private ImagesBank imagesBank;

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
            SoundManager.Instance.PlayClick();
            PlayerManager.SetTeammateCooldown(TeammateType.Manager, GameManager.Instance.Settings.ManagerCooldown);
            workingPage.StartWork(_rapper, isFeat);
            rappersPage.Close();
        }
        
        /// <summary>
        /// Открывает персональную карточку репера
        /// </summary>
        public void Show(RapperInfo rapperInfo)
        {
            _rapper = rapperInfo;
            
            DisplayInfo(rapperInfo);
            CheckPlayerManager();
            
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Отобажает информацию в UI
        /// </summary>
        private void DisplayInfo(RapperInfo info)
        {
            avatar.sprite = info.IsCustom ? customImage : info.Avatar; 
            nickname.text = info.Name;
            vocobulary.text = info.Vocobulary.ToString();
            bitmaking.text = info.Bitmaking.ToString();
            management.text = info.Management.ToString();
            fans.text = $"{info.Fans}M";
        }

        /// <summary>
        /// Вызывается перед открытием страницы
        /// </summary>
        private void CheckPlayerManager()
        {
            var manager = PlayerManager.Data.Team.Manager;

            bool canInteract = TeamManager.IsAvailable(TeammateType.Manager) && manager.Cooldown == 0;
            battleButton.interactable = canInteract;
            featButton.interactable = canInteract;

            managerAvatar.sprite = TeamManager.IsAvailable(TeammateType.Manager)
                ? imagesBank.ProducerActive
                : imagesBank.ProducerInactive;
        }
    }
}