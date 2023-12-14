using System.Collections.Generic;
using System.Linq;
using Core;
using Data;
using Game.UI.ScrollViewController;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Rappers
{
    /// <summary>
    /// Страница реальных реперов
    /// </summary>
    public class RappersPage : Page
    {
        [Space]
        [SerializeField] private ScrollViewController list;
        [SerializeField] private GameObject template;

        [Space]
        [SerializeField] private RapperCard rapperCard;
        [SerializeField] private Button addNewRapperButton;
        [SerializeField] private NewRapperPage newRapperPage;
        
        private List<RapperRow> _listItems = new();

        private void Start()
        {
            addNewRapperButton.onClick.AddListener(OpenNewRapperPage);
        }

        private void OpenNewRapperPage()
        {
            SoundManager.Instance.PlayClick();
            newRapperPage.Open();
            Close();
        }

        protected override void BeforePageOpen()
        {
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
            var allRappers = RappersManager.Instance.GetAllRappers().ToList();
            
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

        protected override void AfterPageOpen()
        {
            TutorialManager.Instance.ShowTutorial("tutorial_rappers");
        }
        
        protected override void AfterPageClose()
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
            RappersManager.Instance.RemoveCustom(customRapper);
            AfterPageClose();
            BeforePageOpen();
        }
    }
}