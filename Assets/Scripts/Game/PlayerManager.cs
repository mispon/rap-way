﻿using System;
using System.Linq;
using Core;
using Core.Interfaces;
using Data;
using Enums;
using Models.Game;
using Models.Player;
using Models.Info.Production;
using UnityEngine;
using Utils;

namespace Game
{
    /// <summary>
    /// Логика взаимодействия с данными игрока
    /// </summary>
    /// TODO: Перейти на MessageBroker и избавиться от этого менеджера
    public class PlayerManager : Singleton<PlayerManager>, IStarter
    {
        /// <summary>
        /// Событие добавления денег
        /// </summary>
        public event Action<int> onMoneyAdd = _ => {};

        /// <summary>
        /// Событие добавления фанатов
        /// </summary>
        public event Action<int> onFansAdd = _ => {};

        /// <summary>
        /// Событие добавления хайпа
        /// </summary>
        public event Action<int> onHypeAdd = _ => {};

        /// <summary>
        /// Данные игрока
        /// </summary>
        public static PlayerData Data { get; private set; }

        /// <summary>
        /// Инициализация объекта
        /// </summary>
        public void OnStart()
        {
            Data = GameManager.Instance.PlayerData;
        }

        /// <summary>
        /// Выдает награду за завершение основного действия
        /// </summary>
        public void GiveReward(int fans, int money, int exp = 0)
        {
            AddFans(fans, exp);
            AddMoney(money);
        }

        /// <summary>
        /// Изменяет количество фанатов
        /// </summary>
        public void AddFans(int fans, int exp = 0)
        {
            AddExp(exp);
            Data.Fans = SafetyAdd(Data.Fans, fans, GameManager.Instance.Settings.MaxFans);
            onFansAdd.Invoke(Data.Fans);
            // fans changed
        }

        /// <summary>
        /// Изменяет количество денег
        /// </summary>
        // TODO: DELETE
        public void AddMoney(int money, int exp = 0)
        {
            AddExp(exp);
            Data.Money = SafetyAdd(Data.Money, money, GameManager.Instance.Settings.MaxMoney);
            onMoneyAdd.Invoke(Data.Money);
        }

        /// <summary>
        /// Изменяет количество хайпа
        /// </summary>
        public void AddHype(int hype)
        {
            int minHype = Data.Goods.Sum(e => e.Hype);
            const int maxHype = 100;

            Data.Hype = Mathf.Clamp(Data.Hype + hype, minHype, maxHype);
            onHypeAdd.Invoke(Data.Hype);
        }

        /// <summary>
        /// Изменяет количество опыта
        /// </summary>
        public void AddExp(int exp)
        {
            Data.Exp += exp;
        }

        /// <summary>
        /// Выполянет оплату
        /// </summary>
        public bool SpendMoney(int money)
        {
            if (Data.Money < money)
                return false;

            SoundManager.Instance.PlaySound(UIActionType.Pay);
            AddMoney(-money);
            return true;
        }

        /// <summary>
        /// Возвращает идентификатор для новой сущности
        /// </summary>
        public static int GetNextProductionId<T>() where T : Production
        {
            var history = Data.History;
            var id = 0;

            if (typeof(T) == typeof(TrackInfo))
                id = history.TrackList.Any() ? history.TrackList.Max(e => e.Id) : 0;

            if (typeof(T) == typeof(ClipInfo))
                id = history.ClipList.Any() ? history.ClipList.Max(e => e.Id) : 0;

            if (typeof(T) == typeof(AlbumInfo))
                id = history.AlbumList.Any() ? history.AlbumList.Max(e => e.Id) : 0;

            if (typeof(T) == typeof(ConcertInfo))
                id = history.ConcertList.Any() ? history.ConcertList.Max(e => e.Id) : 0;

            return id + 1;
        }

        /// <summary>
        /// Устанавливает время отдыха указанному тиммейту
        /// </summary>
        public static void SetTeammateCooldown(TeammateType type, int cooldown)
        {
            var teammate = Data.Team.TeammatesArray.First(e => e.Type == type);
            teammate.Cooldown = cooldown;
        }

        /// <summary>
        /// Обновляет информацию о трендах
        /// </summary>
        public static void UpdateTrends(Styles style, Themes theme)
        {
            if (Data.LastKnownTrends == null)
            {
                Data.LastKnownTrends = Trends.New;
            }

            Data.LastKnownTrends.Style = style;
            Data.LastKnownTrends.Theme = theme;
        }
        
        private static int SafetyAdd(int current, int increment, int maxValue)
        {
            return maxValue - current > increment
                ? current + increment
                : maxValue;
        }
    }
}