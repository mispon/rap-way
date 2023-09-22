using System.Collections.Generic;
using Core;
using Game.UI.ScrollViewController;
using UnityEngine;

namespace Game.Pages.Eagler
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
            TutorialManager.Instance.ShowTutorial("tutorial_eagler");
            //AppodealManager.Instance.ShowBanner();
        }
        
        protected override void AfterPageClose()
        {
            foreach (var eagle in _feedItems)
            {
                Destroy(eagle);
            }
            
            _feedItems.Clear();
            //AppodealManager.Instance.HideBanner();
        }
    }
}