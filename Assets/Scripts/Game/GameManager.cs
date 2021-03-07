using System;
using Core;
using Core.Settings;
using Localization;
using Models.Game;
using Models.Player;
using UnityEngine;
using Utils;

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
        [Header("Игровые настройки")]
        public GameSettings Settings;

        public PlayerData PlayerData { get; private set; }
        public GameStats GameStats { get; private set; }

        [NonSerialized] public bool IsReady;
        
        private void Start()
        {
            LoadApplicationData();
            LocalizationManager.Instance.LoadLocalization(GameStats.Lang, true);
            SoundManager.Instance.Setup(GameStats.SoundVolume, GameStats.MusicVolume);

            IsReady = true;
        }

        /// <summary>
        /// Удаляет игровые сохранения
        /// </summary>
        public void RemoveSaves()
        {
            DataManager.Clear(playersDataKey);
            DataManager.Clear(gameDataKey);
        }

        /// <summary>
        /// Загрузка данных приложения
        /// </summary>
        public void LoadApplicationData()
        {
            PlayerData = DataManager.Load<PlayerData>(playersDataKey);
            GameStats = DataManager.Load<GameStats>(gameDataKey) ?? GameStats.New;
        }

        /// <summary>
        /// Сохранение данных приложения
        /// </summary>
        public void SaveApplicationData()
        {
            if (TimeManager.Instance == null)
                return;

            GameStats.Now = TimeManager.Instance.Now;
            
            DataManager.Save(PlayerData, playersDataKey);
            DataManager.Save(GameStats, gameDataKey);
        }

        private void OnDisable()
        {
            SaveApplicationData();
        }
    }
}