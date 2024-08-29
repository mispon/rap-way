using System.Collections.Generic;
using System.Linq;
using Core;
using Enums;
using Core.Analytics;
using Game.Rappers.Desc;
using MessageBroker;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using UI.Controls.Ask;
using UI.Controls.ScrollViewController;
using UI.Enums;
using UI.Windows.GameScreen.Charts;
using UI.Windows.Tutorial;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;
using RappersAPI = Game.Rappers.RappersPackage;

namespace UI.Windows.GameScreen.Rappers
{
    public class RappersPage : Page
    {
        [SerializeField] private AskingWindow askingWindow;
        [SerializeField] private ScrollViewController list;
        [SerializeField] private GameObject template;

        [Space]
        [SerializeField] private RapperCard rapperCard;
        [SerializeField] private Button addNewRapperButton;

        private readonly List<RapperRow> _listItems = new();

        private void Start()
        {
            addNewRapperButton.onClick.AddListener(OpenNewRapperPage);
        }

        private void OpenNewRapperPage()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.NewRapper));
            base.Hide();
        }

        protected override void BeforeShow(object ctx = null)
        {
            AnalyticsManager.LogEvent(FirebaseGameEvents.RappersPageOpened);

            rapperCard.onDelete += HandleRapperDelete;

            var rappers = GetAllRappers();
            for (var i = 0; i < rappers.Count; i++)
            {
                var info = rappers[i];

                var row = list.InstantiatedElement<RapperRow>(template);
                row.Initialize(i + 1, info);

                _listItems.Add(row);
            }

            list.RepositionElements(_listItems);
        }

        protected override void AfterShow(object ctx = null)
        {
            HintsManager.Instance.ShowHint("tutorial_rappers", ChartsTabType.Rappers);
        }

        /// <summary>
        /// Returns all rappers (internal and custom) sorted desc by fans count
        /// </summary>
        private static List<RapperInfo> GetAllRappers()
        {
            var allRappers = RappersAPI.Instance.GetAll().ToList();

            allRappers.Add(new RapperInfo
            {
                Name = PlayerAPI.Data.Info.NickName,
                Fans = PlayerAPI.Data.Fans / 1_000_000,
                Vocobulary = PlayerAPI.Data.Stats.Vocobulary.Value,
                Bitmaking = PlayerAPI.Data.Stats.Bitmaking.Value,
                Management = PlayerAPI.Data.Stats.Management.Value,
                Label = PlayerAPI.Data.Label,
                IsPlayer = true
            });

            return allRappers.OrderByDescending(r => r.Fans).ToList();
        }

        protected override void AfterHide()
        {
            rapperCard.onDelete -= HandleRapperDelete;

            foreach (var row in _listItems)
            {
                Destroy(row.gameObject);
            }

            _listItems.Clear();
        }

        /// <summary>
        /// Process custom rapper remove
        /// </summary>
        private void HandleRapperDelete(RapperInfo customRapper)
        {
            askingWindow.Show(
                GetLocale("delete_rapper_question"),
                () =>
                {
                    RappersAPI.Instance.RemoveCustom(customRapper);
                    AfterHide();
                    BeforeShow();
                }
            );
        }
    }
}