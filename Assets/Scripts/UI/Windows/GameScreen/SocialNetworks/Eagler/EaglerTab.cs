using System.Collections.Generic;
using Game.SocialNetworks.Eagler;
using UI.Controls.ScrollViewController;
using UI.Windows.Tutorial;
using UnityEngine;

// using Firebase.Analytics;

namespace UI.Windows.GameScreen.SocialNetworks.Eagler
{
    public class EaglerTab : Tab
    {
        [SerializeField] private ScrollViewController feed;
        [SerializeField] private GameObject           template;

        private readonly List<EaglerCard> _feedItems = new();

        protected override void BeforeOpen()
        {
            var eagles = EaglerManager.Instance.GetEagles();

            for (var i = 0; i < eagles.Count; i++)
            {
                var data = eagles[i];

                var eagle = feed.InstantiatedElement<EaglerCard>(template);
                eagle.Initialize(i + 1, data);

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