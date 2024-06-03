using System.Collections.Generic;
using Enums;
// using Firebase.Analytics;
using Game.Socials.Eagler;
using UI.Controls.ScrollViewController;
using UI.Windows.Tutorial;
using UnityEngine;

namespace UI.Windows.GameScreen.Eagler
{
    public class EaglerPage : Page
    {
        [SerializeField] private ScrollViewController feed;
        [SerializeField] private GameObject template;
        
        private readonly List<EagleCard> _feedItems = new();
        
        protected override void BeforeShow(object ctx = null)
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
        
        protected override void AfterShow(object ctx = null)
        {
            HintsManager.Instance.ShowHint("tutorial_eagler");
            // FirebaseAnalytics.LogEvent(FirebaseGameEvents.TwitterOpened);
        }
        
        protected override void AfterHide()
        {
            foreach (var eagle in _feedItems)
            {
                Destroy(eagle.gameObject);
            }
            _feedItems.Clear();
        }
    }
}