using System;
using System.Collections.Generic;
using Data;
using Localization;
using Models.Game;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Core
{
    public class EaglerManager : Singleton<EaglerManager>
    {
        [SerializeField] private EaglerData data;

        private List<Eagle> _eagles = new();

        /// <summary>
        /// Возвращает список постов
        /// </summary>
        public List<Eagle> GetEagles() => _eagles;

        /// <summary>
        /// Добавляет новый пост от игрока
        /// </summary>
        public void CreateUserEagle(string nickname, string message, int likes)
        {
            var eagle = new Eagle
            {
                Date = TimeManager.Instance.Now,
                Nickname = nickname,
                Message = message,
                Likes = likes,
                Views = CalcViews(likes),
                Shares = CalcShares(likes)
            };
            
            AddEagle(eagle);
        }

        /// <summary>
        /// Создает три случайных твита
        /// </summary>
        public List<Eagle> GenerateEagles(float quality, int fans)
        {
            var eagles = new List<Eagle>(3);

            for (int i = 0; i < 3; i++)
            {
                var eagle = GenerateRandomEagle(quality, fans);
                AddEagle(eagle);
            }
            
            return eagles;
        }

        private Eagle GenerateRandomEagle(float quality, int fans)
        {
            var likes = CalcLikes(fans);
            
            var dice = Random.Range(0, 1);
            var messageKey = dice > quality 
                ? $"{data.NegativePostKey}_{Random.Range(0, data.NegativePostsCount)}"
                : $"{data.PositivePostKey}_{Random.Range(0, data.PositivePostsCount)}";
            var message = LocalizationManager.Instance.Get(messageKey);
            
            var nickname = data.Nicknames[Random.Range(0, data.Nicknames.Length)];
            
            return new Eagle
            {
                Date = TimeManager.Instance.Now,
                Nickname = nickname,
                Message = message,
                Likes = likes,
                Views = CalcViews(likes),
                Shares = CalcShares(likes)
            };
        }

        private void AddEagle(Eagle eagle)
        {
            _eagles.Insert(0, eagle);
        }

        private static int CalcViews(int likes)
        {
            var views = likes * 10 + 1;
            views = Math.Min(100, views);
            
            views = Random.Range(views - likes, views + likes);
            
            return views;
        }

        private static int CalcShares(int likes)
        {
            var shares = likes / 2 + 1;
            shares = Math.Min(1, shares);
            
            var fuzz = GetPercentOf(shares, 20);
            shares = Random.Range(shares - fuzz, shares + fuzz);
            
            return shares;
        }

        private static int CalcLikes(int fans)
        {
            var likes =  GetPercentOf(fans, 10);
            likes = Math.Min(10, likes);

            var fuzz = GetPercentOf(likes, 20);
            likes = Random.Range(likes - fuzz, likes + fuzz);
            
            return likes;
        }

        private static int GetPercentOf(int value, int percent)
        {
            return (value / 100 * percent) + 1;
        }
    }
}