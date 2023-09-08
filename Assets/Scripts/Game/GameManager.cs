using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Settings;
using Data;
using Localization;
using Models.CustomRappers;
using Models.Game;
using Models.Player;
using UnityEngine;
using Utils;
using Utils.Extensions;

namespace Game
{
    /// <summary>
    /// Логика управления состоянием игры
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        [Header("Ключи сохранения данных")]
        [SerializeField] private string playersDataKey;
        [SerializeField] private string gameDataKey;
        [SerializeField] private string customRappersDataKey;
        [Header("Игровые настройки")]
        public GameSettings Settings;

        [Header("GAME STATE")]
        public PlayerData PlayerData;
        public GameStats GameStats;
        public List<RapperInfo> CustomRappers;

        [NonSerialized] public bool IsReady;

        private void Start()
        {
            LoadApplicationData();
            LocalizationManager.Instance.LoadLocalization(GameStats.Lang, true);
            SoundManager.Instance.Setup(GameStats.SoundVolume, GameStats.MusicVolume);

            IsReady = true;
        }

        /// <summary>
        /// Создает новый объект персонажа
        /// </summary>
        public PlayerData CreateNewPlayer()
        {
            DataManager.Clear(playersDataKey);
            PlayerData = PlayerData.New;
            return PlayerData;
        }

        /// <summary>
        /// Удаляет игровые сохранения
        /// </summary>
        public void RemoveSaves()
        {
            DataManager.Clear(playersDataKey);
            DataManager.Clear(gameDataKey);
            DataManager.Clear(customRappersDataKey);
        }

        /// <summary>
        /// Загрузка данных приложения
        /// </summary>
        private void LoadApplicationData()
        {
            PlayerData = DataManager.Load<PlayerData>(playersDataKey) ?? PlayerData.New;
            GameStats = DataManager.Load<GameStats>(gameDataKey) ?? GameStats.New;
            
            var customRappers = DataManager.Load<CustomRappersInfo>(customRappersDataKey) ?? new CustomRappersInfo();
            CustomRappers = customRappers.Values?.ToList() ?? new List<RapperInfo>();
        }

        /// <summary>
        /// Сохранение данных приложения
        /// </summary>
        public void SaveApplicationData()
        {
            if (TimeManager.Instance != null)
            {
                GameStats.Now = TimeManager.Instance.Now.DateToString();
            }

            DataManager.Save(PlayerData, playersDataKey);
            DataManager.Save(GameStats, gameDataKey);

            var customRappers = new CustomRappersInfo { Values = CustomRappers.ToArray() };
            DataManager.Save(customRappers, customRappersDataKey);
        }

        /// <summary>
        /// Проверяет, создан ли персонаж
        /// </summary>
        public bool HasCharacter()
        {
            return !string.IsNullOrWhiteSpace(PlayerData.Info.NickName);
        }

        private void OnApplicationQuit()
        {
            SaveApplicationData();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveApplicationData();    
            }
        }
    }
}