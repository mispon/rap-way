using System;
using Core;
using Enums;
using Firebase.Analytics;
using Game;
using Game.Player;
using Game.Player.Team;
using Game.Rappers.Desc;
using ScriptableObjects;
using UI.Windows.Pages.Charts;
using UnityEngine;
using UnityEngine.UI;
using RappersAPI =  Game.Rappers.RappersPackage;
using LabelsAPI = Game.Labels.LabelsPackage;

namespace UI.Windows.Pages.Rappers
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
        [SerializeField] private Button labelButton;
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
            battleButton.onClick.AddListener(() => StartConversation(ConversationType.Battle));
            featButton.onClick.AddListener(() => StartConversation(ConversationType.Feat));
            labelButton.onClick.AddListener(() => StartConversation(ConversationType.Label));
            deleteButton.onClick.AddListener(DeleteRapper);
        }

        /// <summary>
        /// Обработчик кнопок
        /// </summary>
        private void StartConversation(ConversationType convType)
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            
            switch (convType)
            {
                case ConversationType.Battle:
                    FirebaseAnalytics.LogEvent(FirebaseGameEvents.RapperBattleAction);
                    break;
                case ConversationType.Feat:
                    FirebaseAnalytics.LogEvent(FirebaseGameEvents.RapperFeatAction);
                    break;
            }

            int cooldown = GameManager.Instance.Settings.Team.ManagerCooldown;
            PlayerManager.SetTeammateCooldown(TeammateType.Manager, cooldown);
            
            workingPage.StartWork(_rapper, convType);
            
            rappersPage.Close();
            chartsPage.Hide();
        }

        private void DeleteRapper()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
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
            CheckPlayersLabel();
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

            bool labelsButtonActive = !info.IsPlayer && 
                                      !LabelsAPI.Instance.IsPlayerLabelEmpty &&
                                      !string.Equals(_rapper.Label, LabelsAPI.Instance.PlayerLabel.Name, StringComparison.InvariantCultureIgnoreCase);
            labelButton.gameObject.SetActive(labelsButtonActive);
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

        private void CheckPlayersLabel()
        {
            var manager = PlayerManager.Data.Team.Manager;
            
            bool canInteract = TeamManager.IsAvailable(TeammateType.Manager) && manager.Cooldown == 0;

            var labelInfo = LabelsAPI.Instance.Get(PlayerManager.Data.Label);
            if (labelInfo is {IsPlayer: true, IsFrozen: false})
            {
                float prestige = RappersAPI.GetRapperPrestige(_rapper);
                canInteract &= Mathf.Abs(prestige - labelInfo.Prestige.Value) <= 1.5f;
            } else
            {
                canInteract = false;
            }
            
            labelButton.interactable = canInteract;
        }
    }
}