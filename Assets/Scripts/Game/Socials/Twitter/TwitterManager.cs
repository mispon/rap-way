using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Game.Time;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;
using PlayerAPI = Game.Player.PlayerPackage;

namespace Game.Socials.Twitter
{
    /// <summary>
    /// TODO: replace with message broker
    /// </summary>
    public class TwitterManager : Singleton<TwitterManager>
    {
        [SerializeField] private TwitData data;

        public List<Twit> GetTwits() => GameManager.Instance.Twits.Take(20).ToList();
        
        /// <summary>
        /// Добавляет новый пост от игрока
        /// </summary>
        public void CreateUserTwit(string nickname, string message, int likes)
        {
            var eagle = new Twit
            {
                Date = TimeManager.Instance.DisplayNow,
                Nickname = nickname,
                Message = message,
                Likes = likes,
                Views = CalcViews(likes),
                Shares = CalcShares(likes),
                IsUser = true
            };
            
            AddTwit(eagle);
        }
        
        public List<Twit> GenerateTwits(float quality)
        {
            var twits = new List<Twit>(3);

            var fans = PlayerAPI.Data.Fans;
            for (int i = 0; i < 3; i++)
            {
                var twit = GenerateRandomTwit(quality, fans);
                AddTwit(twit);
                twits.Add(twit);
            }
            
            return twits;
        }

        private Twit GenerateRandomTwit(float quality, int fans)
        {
            var likes = CalcLikes(fans);
            
            var dice = Random.Range(0f, 1f);
            var messageKey = dice > quality 
                ? $"{data.NegativePostKey}_{Random.Range(0, data.NegativePostsCount)}"
                : $"{data.PositivePostKey}_{Random.Range(0, data.PositivePostsCount)}";
            
            var nickname = data.Nicknames[Random.Range(0, data.Nicknames.Length)];
            var playerName = PlayerAPI.Data.Info.NickName;
            var randomTag = data.Hashtags[Random.Range(0,  data.Hashtags.Length)];
            
            return new Twit
            {
                Date = TimeManager.Instance.DisplayNow,
                Nickname = nickname,
                Message = messageKey,
                Likes = likes,
                Views = CalcViews(likes),
                Shares = CalcShares(likes),
                Tags = $" <color=#109c22>#{playerName}</color> <color=#109c22>#{randomTag}</color>"
            };
        }

        private static void AddTwit(Twit eagle)
        {
            GameManager.Instance.Twits.Insert(0, eagle);
        }

        private static int CalcViews(int likes)
        {
            var views = likes * 10 + 1;
            views = Math.Max(100, views);
            
            views = Random.Range(views - likes, views + likes);
            
            return views;
        }

        private static int CalcShares(int likes)
        {
            var shares = likes / 2 + 1;
            shares = Math.Max(1, shares);
            
            var fuzz = GetPercentOf(shares, 20);
            shares = Random.Range(shares - fuzz, shares + fuzz);
            
            return shares;
        }

        private static int CalcLikes(int fans)
        {
            var likes =  GetPercentOf(fans, 1);
            likes = Math.Max(10, likes);

            var fuzz = GetPercentOf(likes, 20);
            likes = Random.Range(likes - fuzz, likes + fuzz);
            
            return likes;
        }

        private static int GetPercentOf(int value, int percent)
        {
            return value / 100 * percent + 1;
        }
    }
}