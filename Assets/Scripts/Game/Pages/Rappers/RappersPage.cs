using System;
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
        [Header("Данные об исполнителях")]
        [SerializeField] private RappersData data;
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
        private List<RapperInfo> GetAllRappers()
        {
            int rappersTotal = data.Rappers.Count + GameManager.Instance.CustomRappers.Count;
            var allRappers = new List<RapperInfo>(rappersTotal);
                
            foreach (var rapperInfo in data.Rappers)
            {
                allRappers.Add(rapperInfo);
            }
            foreach (var rapperInfo in GameManager.Instance.CustomRappers)
            {
                allRappers.Add(rapperInfo);
            }

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
            GameManager.Instance.CustomRappers.Remove(customRapper);
            AfterPageClose();
            BeforePageOpen();
        }
    }
}