using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Extensions;
using Game.Rappers.Desc;
using Game.Time;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.SocialNetworks.Eagler
{
    public class EaglerManager : Singleton<EaglerManager>
    {
        [SerializeField] private EaglerData data;

        public List<Eagle> GetEagles()
        {
            return GameManager.Instance.Eagles.Take(20).ToList();
        }

        public string[] GetTrends(int count = 5)
        {
            return data.Hashtags
                .Shuffle()
                .Take(count)
                .ToArray();
        }

        public void CreateUserEagle(string nickname, string message, int likes)
        {
            var eagle = new Eagle
            {
                Date     = TimeManager.Instance.DisplayNow,
                Nickname = nickname,
                Message  = message,
                Likes    = likes,
                Views    = CalcViews(likes),
                Shares   = CalcShares(likes),
                IsUser   = true
            };

            AddEagle(eagle);
        }

        public void CreateRapperEagle(RapperInfo rapper, string target)
        {
            var likes = CalcLikes(rapper.Fans);

            var dice = Random.Range(0f, 1f);
            var messageKey = dice >= 0.5f
                ? $"{data.RapperNegativePostKey}_{Random.Range(0, data.RapperNegativePostsCount)}"
                : $"{data.RapperPositivePostKey}_{Random.Range(0, data.RapperPositivePostsCount)}";

            var eagle = new Eagle
            {
                Date     = TimeManager.Instance.DisplayNow,
                Nickname = rapper.Name,
                Likes    = likes,
                Views    = CalcViews(likes),
                Shares   = CalcShares(likes),
                Message  = messageKey,
                Tags     = $" <color=#109c22>#{target}</color>"
            };

            AddEagle(eagle);
        }

        public List<Eagle> GenerateEagles(int count, string rapperName, int fans, float quality)
        {
            var eagles = new List<Eagle>(count);

            for (var i = 0; i < count; i++)
            {
                var eagle = GenerateRandomEagle(rapperName, fans, quality);
                AddEagle(eagle);
                eagles.Add(eagle);
            }

            return eagles;
        }

        private Eagle GenerateRandomEagle(string rapperName, int fans, float quality)
        {
            var likes = CalcLikes(fans);

            var dice = Random.Range(0f, 1f);
            var messageKey = dice > quality
                ? $"{data.NegativePostKey}_{Random.Range(0, data.NegativePostsCount)}"
                : $"{data.PositivePostKey}_{Random.Range(0, data.PositivePostsCount)}";

            var nickname  = data.Nicknames[Random.Range(0, data.Nicknames.Length)];
            var randomTag = data.Hashtags[Random.Range(0, data.Hashtags.Length)];

            return new Eagle
            {
                Date     = TimeManager.Instance.DisplayNow,
                Nickname = nickname,
                Message  = messageKey,
                Likes    = likes,
                Views    = CalcViews(likes),
                Shares   = CalcShares(likes),
                Tags     = $" <color=#109c22>#{rapperName}</color> <color=#109c22>#{randomTag}</color>"
            };
        }

        private static void AddEagle(Eagle eagle)
        {
            GameManager.Instance.Eagles.Insert(0, eagle);
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
            var likes = GetPercentOf(fans, 1);
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