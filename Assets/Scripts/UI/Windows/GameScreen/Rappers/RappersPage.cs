using System.Collections.Generic;
using System.Linq;
using Core;
using Enums;
using Firebase.Analytics;
using Game.Player;
using Game.Rappers.Desc;
using MessageBroker;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using UI.Controls.Ask;
using UI.Controls.ScrollViewController;
using UI.Enums;
using UI.Windows.Tutorial;
using UnityEngine;
using UnityEngine.UI;
using RappersAPI =  Game.Rappers.RappersPackage;

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

        private static void OpenNewRapperPage()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.NewRapper));
        }

        protected override void BeforeShow()
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.RappersPageOpened);
            
            rapperCard.onDelete += HandleRapperDelete;
            
            var rappers = GetAllRappers();
            for (var i = 0; i < rappers.Count; i++)
            {
                var info = rappers[i];
                
                var row = list.InstantiatedElement<RapperRow>(template);
                row.Initialize(i+1, info);
                
                _listItems.Add(row);
            }

            list.RepositionElements(_listItems);
        }

        /// <summary>
        /// Returns all rappers (internal and custom) sorted desc by fans count
        /// </summary>
        private static List<RapperInfo> GetAllRappers()
        {
            var allRappers = RappersAPI.Instance.GetAll().ToList();
            
            allRappers.Add(new RapperInfo
            {
                Name = PlayerManager.Data.Info.NickName,
                Fans = PlayerManager.Data.Fans / 1_000_000,
                Vocobulary = PlayerManager.Data.Stats.Vocobulary.Value,
                Bitmaking = PlayerManager.Data.Stats.Bitmaking.Value,
                Management = PlayerManager.Data.Stats.Management.Value,
                Label = PlayerManager.Data.Label,
                IsPlayer = true
            });

            return allRappers.OrderByDescending(r => r.Fans).ToList();
        }

        protected override void AfterShow()
        {
            HintsManager.Instance.ShowHint("tutorial_rappers");
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
                () => {
                    RappersAPI.Instance.RemoveCustom(customRapper);
                    AfterHide();
                    BeforeShow();
                }
            );
        }
    }
}