using System;
using System.Collections.Generic;
using Core;
using Core.Analytics;
using Enums;
using Extensions;
using Game;
using Game.Player.Team;
using Game.Rappers.Desc;
using MessageBroker;
using MessageBroker.Messages.Player;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using UI.Enums;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;
using RappersAPI = Game.Rappers.RappersPackage;
using LabelsAPI = Game.Labels.LabelsPackage;

namespace UI.Windows.GameScreen.Rappers
{
    public class RapperCard : MonoBehaviour
    {
        [Header("Rapper Card")]
        [SerializeField] private Sprite customImage;
        [SerializeField] private Sprite playerMaleImage;
        [SerializeField] private Sprite playerFemaleImage;
        [SerializeField] private Image  avatar;
        [SerializeField] private Text   nickname;

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

        [Header("Images Bank")]
        [SerializeField] private ImagesBank imagesBank;

        public event Action<RapperInfo> onDelete = _ => { };

        private RapperInfo _rapper;

        private void Start()
        {
            battleButton.onClick.AddListener(() => StartConversation(ConversationType.Battle));
            featButton.onClick.AddListener(() => StartConversation(ConversationType.Feat));
            labelButton.onClick.AddListener(() => StartConversation(ConversationType.Label));
            deleteButton.onClick.AddListener(DeleteRapper);
        }

        private void StartConversation(ConversationType convType)
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);

            switch (convType)
            {
                case ConversationType.Battle:
                    AnalyticsManager.LogEvent(FirebaseGameEvents.RapperBattleAction);
                    break;
                case ConversationType.Feat:
                    AnalyticsManager.LogEvent(FirebaseGameEvents.RapperFeatAction);
                    break;
                case ConversationType.Label:
                    AnalyticsManager.LogEvent(FirebaseGameEvents.RapperLabelAction);
                    break;
            }

            var manager     = PlayerAPI.Data.Team.Manager;
            var isAvailable = TeamManager.IsAvailable(TeammateType.Manager) && manager.Cooldown == 0;
            if (!isAvailable)
            {
                Refresh();
                return;
            }

            var cooldown = GameManager.Instance.Settings.Team.ManagerCooldown;
            MsgBroker.Instance.Publish(new TeammateCooldownMessage
            {
                Type     = TeammateType.Manager,
                Cooldown = cooldown
            });

            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type = WindowType.RapperConversationsWork,
                Context = new Dictionary<string, object>
                {
                    ["rapper"]    = _rapper,
                    ["conv_type"] = convType
                }
            });

            Refresh();
        }

        private void DeleteRapper()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            onDelete.Invoke(_rapper);
        }

        public void Show(RapperInfo rapperInfo)
        {
            _rapper = rapperInfo;

            deleteButton.gameObject.SetActive(_rapper.IsCustom);

            DisplayInfo(rapperInfo);
            Refresh();
        }

        private void DisplayInfo(RapperInfo info)
        {
            avatar.sprite   = GetAvatar(info);
            nickname.text   = info.Name;
            vocobulary.text = info.Vocobulary.ToString();
            bitmaking.text  = info.Bitmaking.ToString();
            management.text = info.Management.ToString();
            fans.text       = info.Fans.GetShort();
            label.text      = info.Label != "" ? info.Label : "-";

            featButton.gameObject.SetActive(!info.IsPlayer);
            battleButton.gameObject.SetActive(!info.IsPlayer);

            var labelsButtonActive = !info.IsPlayer &&
                                     !LabelsAPI.Instance.IsPlayerLabelEmpty &&
                                     !string.Equals(
                                         _rapper.Label,
                                         LabelsAPI.Instance.PlayerLabel.Name,
                                         StringComparison.InvariantCultureIgnoreCase
                                     );
            labelButton.gameObject.SetActive(labelsButtonActive);
        }

        private Sprite GetAvatar(RapperInfo info)
        {
            if (info.IsPlayer)
            {
                return SpritesManager.Instance.GetPortrait(info.Name);
            }

            return info.IsCustom || info.Avatar == null ? customImage : info.Avatar;
        }

        private void Refresh()
        {
            CheckPlayerManager();
            CheckPlayersLabel();
        }

        private void CheckPlayerManager()
        {
            var manager = PlayerAPI.Data.Team.Manager;

            var canInteract = TeamManager.IsAvailable(TeammateType.Manager) && manager.Cooldown == 0;
            battleButton.interactable = canInteract;
            featButton.interactable   = canInteract;

            managerAvatar.sprite = TeamManager.IsAvailable(TeammateType.Manager)
                ? imagesBank.ProducerActive
                : imagesBank.ProducerInactive;
        }

        private void CheckPlayersLabel()
        {
            var manager = PlayerAPI.Data.Team.Manager;

            var canInteract = TeamManager.IsAvailable(TeammateType.Manager) && manager.Cooldown == 0;

            var labelInfo = LabelsAPI.Instance.Get(PlayerAPI.Data.Label);
            if (labelInfo is {IsPlayer: true, IsFrozen: false})
            {
                var prestige = RappersAPI.GetRapperPrestige(_rapper);
                canInteract &= Mathf.Abs(prestige - labelInfo.Prestige.Value) <= 1.5f;
            } else
            {
                canInteract = false;
            }

            labelButton.interactable = canInteract;
        }
    }
}