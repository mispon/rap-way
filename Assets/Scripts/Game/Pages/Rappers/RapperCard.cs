using System;
using Core;
using Data;
using Enums;
using Game.Pages.Charts;
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
        [SerializeField] private Sprite playerMaleImage;
        [SerializeField] private Sprite playerFemaleImage;
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
        [SerializeField] private Button deleteButton;
        [Space]
        [SerializeField] private Text fans;
        [SerializeField] private Text label;

        [Header("Страницы")]
        [SerializeField] private ChartsPage chartsPage;
        [SerializeField] private RappersPage rappersPage;
        [SerializeField] private RapperWorkingPage workingPage;

        [Header("Банк картинок")]
        [SerializeField] private ImagesBank imagesBank;

        public event Action<RapperInfo> onDelete = _ => {};
        
        private RapperInfo _rapper;

        private void Start()
        {
            battleButton.onClick.AddListener(() => StartConversation(false));
            featButton.onClick.AddListener(() => StartConversation(true));
            deleteButton.onClick.AddListener(DeleteRapper);
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
            chartsPage.Hide();
        }

        private void DeleteRapper()
        {
            SoundManager.Instance.PlayClick();
            onDelete.Invoke(_rapper);
        }
        
        /// <summary>
        /// Открывает персональную карточку репера
        /// </summary>
        public void Show(RapperInfo rapperInfo)
        {
            _rapper = rapperInfo;
            
            deleteButton.gameObject.SetActive(_rapper.IsCustom);
            
            DisplayInfo(rapperInfo);
            CheckPlayerManager();
            
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Отобажает информацию в UI
        /// </summary>
        private void DisplayInfo(RapperInfo info)
        {
            avatar.sprite = GetAvatar(info);
            nickname.text = info.Name;
            vocobulary.text = info.Vocobulary.ToString();
            bitmaking.text = info.Bitmaking.ToString();
            management.text = info.Management.ToString();
            fans.text = $"{info.Fans}M";
            label.text = info.Label != "" ? info.Label : "-";
            
            featButton.gameObject.SetActive(!info.IsPlayer);
            battleButton.gameObject.SetActive(!info.IsPlayer);
        }

        private Sprite GetAvatar(RapperInfo info)
        {
            if (info.IsPlayer)
            {
                return PlayerManager.Data.Info.Gender == Gender.Male
                    ? playerMaleImage
                    : playerFemaleImage;
            }
            
            return info.IsCustom || info.Avatar == null ? customImage : info.Avatar;
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