using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Analytics;
using Core.Localization;
using Enums;
using Game.Labels.Desc;
using Game.Rappers.Desc;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using ScriptableObjects;
using UI.Controls.Ask;
using UI.Controls.ScrollViewController;
using UI.Windows.GameScreen.Labels;
using UI.Windows.Tutorial;
using UnityEngine;
using UnityEngine.UI;
using RappersAPI = Game.Rappers.RappersPackage;
using LabelsAPI = Game.Labels.LabelsPackage;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Personal.LabelTab
{
    public class GameLabelSubTab : Tab
    {
        [SerializeField] private LabelTab     labelTab;
        [SerializeField] private AskingWindow askingWindow;

        [Space]
        [SerializeField] private Image logo;
        [SerializeField] private Text          labelName;
        [SerializeField] private Text          labelDesc;
        [SerializeField] private Text          production;
        [SerializeField] private PrestigeStars stars;
        [SerializeField] private Button        leaveButton;
        [Space]
        [SerializeField] private ScrollViewController list;
        [SerializeField] private GameObject template;

        private readonly List<LabelMemberRow> _listItems = new();

        private void Start()
        {
            leaveButton.onClick.AddListener(LeaveLabel);
        }

        public void Show(LabelInfo label)
        {
            DisplayInfo(label);
            DisplayMembers(label);

            base.Open();

            HintsManager.Instance.ShowHint("tutorial_game_label", PersonalTabType.Label);
        }

        private void DisplayInfo(LabelInfo label)
        {
            logo.sprite     = label.Logo;
            labelName.text  = label.Name;
            labelDesc.text  = LocalizationManager.Instance.Get(label.Desc);
            production.text = label.Production.Value.ToString();

            var prestige = LabelsAPI.Instance.GetPrestige(label);
            stars.Display(prestige);
        }

        private void DisplayMembers(LabelInfo label)
        {
            var members = GetMembers(label.Name);

            for (var i = 0; i < members.Count; i++)
            {
                var info = members[i];

                var row = list.InstantiatedElement<LabelMemberRow>(template);
                row.Initialize(i + 1, info);

                _listItems.Add(row);
            }

            list.RepositionElements(_listItems);
        }

        /// <summary>
        ///     Returns all label members sorted desc by fans count
        /// </summary>
        private static List<RapperInfo> GetMembers(string labelName)
        {
            var members = RappersAPI.Instance
                .GetAll()
                .Where(e => e.Label == labelName)
                .ToList();

            members.Add(new RapperInfo
            {
                Name     = PlayerAPI.Data.Info.NickName,
                Fans     = PlayerAPI.Data.Fans,
                Label    = PlayerAPI.Data.Label,
                IsPlayer = true
            });

            return members.OrderByDescending(r => r.Fans).ToList();
        }

        private void LeaveLabel()
        {
            AnalyticsManager.LogEvent(FirebaseGameEvents.LabelLeaveAction);
            SoundManager.Instance.PlaySound(UIActionType.Click);

            var nickname = PlayerAPI.Data.Info.NickName;

            askingWindow.Show(
                LocalizationManager.Instance.Get("leave_label_question").ToUpper(),
                () =>
                {
                    MsgBroker.Instance.Publish(new NewsMessage
                    {
                        Text       = "news_player_leave_label",
                        TextArgs   = new[] {nickname, PlayerAPI.Data.Label},
                        Sprite     = SpritesManager.Instance.GetPortrait(nickname),
                        Popularity = PlayerAPI.Data.Fans
                    });

                    PlayerAPI.Data.Label = "";
                    labelTab.Reload();
                }
            );
        }

        public override void Close()
        {
            foreach (var row in _listItems)
            {
                Destroy(row.gameObject);
            }

            _listItems.Clear();

            base.Close();
        }
    }
}