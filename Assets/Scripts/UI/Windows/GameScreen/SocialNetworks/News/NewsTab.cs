using System.Collections.Generic;
using System.Linq;
using Game.SocialNetworks.News;
using UI.Controls.ScrollViewController;
using UnityEngine;

namespace UI.Windows.GameScreen.SocialNetworks.News
{
    public class NewsTab : Tab
    {
        [Header("News")]
        [SerializeField] private ScrollViewController newsFeed;
        [SerializeField] private GameObject newsTemplate;

        [Header("Hot News")]
        [SerializeField] private ScrollViewController hotNewsFeed;
        [SerializeField] private GameObject hotNewsTemplate;

        private readonly List<NewsCard> _newsCards = new();
        private readonly List<HotNewsCard> _hotNewsCards = new();

        protected override void BeforeOpen()
        {
            // Fill news feed
            var allNews = NewsManager.Instance.GetNews().ToArray();
            for (var i = 0; i < allNews.Length; i++)
            {
                var data = allNews[i];

                var card = newsFeed.InstantiatedElement<NewsCard>(newsTemplate);
                card.Initialize(i + 1, data);

                _newsCards.Add(card);
            }
            newsFeed.RepositionElements(_newsCards);

            // Fill HOT news feed
            var hotNews = allNews
                .OrderByDescending(e => e.Popularity)
                .Take(5)
                .ToArray();

            for (var i = 0; i < hotNews.Length; i++)
            {
                var data = hotNews[i];

                var card = hotNewsFeed.InstantiatedElement<HotNewsCard>(hotNewsTemplate);
                card.Initialize(i + 1, data);

                _hotNewsCards.Add(card);
            }
            hotNewsFeed.RepositionElements(_hotNewsCards);
        }

        protected override void AfterClose()
        {
            ClearNews(_newsCards);
            ClearNews(_hotNewsCards);
        }

        private void ClearNews<T>(List<T> newsCards) where T : MonoBehaviour
        {
            foreach (var news in newsCards)
            {
                Destroy(news.gameObject);
            }
            newsCards.Clear();
        }
    }
}