using System;
using System.Collections.Generic;
using Core;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using UniRx;
using UnityEngine;

namespace Game.SocialNetworks.News
{
    public class NewsManager : Singleton<NewsManager>
    {
        private readonly CompositeDisposable _disposables = new();

        private void Start()
        {
            MsgBroker.Instance
                .Receive<NewsMessage>()
                .Subscribe(AddNews)
                .AddTo(_disposables);
        }

        public IEnumerable<News> GetNews()
        {
            foreach (var news in GameManager.Instance.News)
            {
                if (SpritesManager.Instance.TryGetByName(news.SpriteName, out var sprite))
                {
                    news.Sprite = sprite;
                }

                yield return news;
            }
        }

        public void AddNews(NewsMessage msg)
        {
            var news = new News
            {
                Text = msg.Text,
                TextArgs = msg.TextArgs,
                Popularity = msg.Popularity,
                Sprite = msg.Sprite,
                Date = TimeManager.Instance.DisplayNow
            };

            if (msg.Sprite != null)
            {
                news.SpriteName = msg.Sprite.name;
            }

            GameManager.Instance.News.Insert(0, news);
        }
    }
}