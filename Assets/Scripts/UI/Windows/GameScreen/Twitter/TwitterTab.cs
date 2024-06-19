using System.Collections.Generic;
using Game.Socials.Twitter;
using UI.Controls.ScrollViewController;
using UI.Windows.Tutorial;
using UnityEngine;
// using Firebase.Analytics;

namespace UI.Windows.GameScreen.Twitter
{
    public class TwitterTab : Tab
    {
        [SerializeField] private ScrollViewController feed;
        [SerializeField] private GameObject template;
        
        private readonly List<TwitterCard> _feedItems = new();
        
        protected override void BeforeOpen()
        {
            var eagles = TwitterManager.Instance.GetTwits();
            
            for (var i = 0; i < eagles.Count; i++)
            {
                var data = eagles[i];
                
                var eagle = feed.InstantiatedElement<TwitterCard>(template);
                eagle.Initialize(i+1, data);
                
                _feedItems.Add(eagle);
            }

            feed.RepositionElements(_feedItems);
        }
        
        protected override void AfterOpen()
        {
            HintsManager.Instance.ShowHint("tutorial_eagler");
            // FirebaseAnalytics.LogEvent(FirebaseGameEvents.TwitterOpened);
        }
        
        protected override void AfterClose()
        {
            foreach (var eagle in _feedItems)
            {
                Destroy(eagle.gameObject);
            }
            _feedItems.Clear();
        }
    }
}