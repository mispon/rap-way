using System.Collections.Generic;
using Enums;
using Firebase.Analytics;
using Game.Player;
using Game.Socials;
using Game.Socials.Eagler;
using UI.Controls.ScrollViewController;
using UI.Windows.GameScreen;
using UI.Windows.Tutorial;
using UnityEngine;

namespace UI.Windows.Pages.Eagler
{
    public class EaglerPage : Page
    {
        [SerializeField] private ScrollViewController feed;
        [SerializeField] private GameObject template;
        
        private List<EagleCard> _feedItems = new();
        
        protected override void BeforePageOpen()
        {
            var eagles = EaglerManager.Instance.GetEagles();
            
            for (var i = 0; i < eagles.Count; i++)
            {
                var data = eagles[i];
                
                var eagle = feed.InstantiatedElement<EagleCard>(template);
                eagle.Initialize(i+1, data);
                
                _feedItems.Add(eagle);
            }

            feed.RepositionElements(_feedItems);
        }
        
        protected override void AfterPageOpen()
        {
            HintsManager.Instance.ShowHint("tutorial_eagler");
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.TwitterOpened);
        }
        
        protected override void AfterPageClose()
        {
            foreach (var eagle in _feedItems)
            {
                Destroy(eagle.gameObject);
            }
            _feedItems.Clear();
        }
    }
}