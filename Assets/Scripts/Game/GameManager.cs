using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Settings;
using Data;
using Localization;
using Models.Game;
using Models.Player;
using UnityEngine;
using Utils;
using Utils.Extensions;

namespace Game
{
    [Serializable]
    public class GameData
    {
        public PlayerData PlayerData;
        public GameStats GameStats;
        public RapperInfo[] CustomRappers;
        public Eagle[] Eagles;
        public string[] ShowedTutorials;
    }
    
    /// <summary>
    /// Логика управления состоянием игры
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        [Header("Урлы в сторах")]
        [SerializeField] private string appStoreURL;
        [SerializeField] private string googlePlayURL;
        
        [Header("Ключ сохранения данных")]
        [SerializeField] private string gameDataKey;
        [Header("Игровые настройки")]
        public GameSettings Settings;

        [Header("GAME STATE")]
        public PlayerData PlayerData;
        public GameStats GameStats;
        public List<RapperInfo> CustomRappers;
        public List<Eagle> Eagles;
        public HashSet<string> ShowedTutorials;

        [NonSerialized] public bool IsReady;

        private void Start()
        {
            LoadApplicationData();
            LocalizationManager.Instance.LoadLocalization(GameStats.Lang, true);
            SoundManager.Instance.Setup(GameStats.SoundVolume, GameStats.MusicVolume);

            IsReady = true;
        }

        public string GetStoreURL()
        {
#if UNITY_ANDROID
            return googlePlayURL;
#elif UNITY_IPHONE
            return appStoreURL;
#else
            return "";
#endif
        }

        /// <summary>
        /// Создает новый объект персонажа
        /// </summary>
        public PlayerData CreateNewPlayer()
        {
            PlayerData = PlayerData.New;
            Eagles = new List<Eagle>(0);
            
            return PlayerData;
        }

        /// <summary>
        /// Удаляет игровые сохранения
        /// </summary>
        public void RemoveSaves()
        {
            DataManager.Clear(gameDataKey);
        }

        /// <summary>
        /// Загрузка данных приложения
        /// </summary>
        private void LoadApplicationData()
        {
            var gameData = DataManager.Load<GameData>(gameDataKey) ?? new GameData
            {
                PlayerData = PlayerData.New,
                GameStats = GameStats.New,
                CustomRappers = Array.Empty<RapperInfo>(),
                Eagles = Array.Empty<Eagle>(),
                ShowedTutorials = Array.Empty<string>()
            };
            
            PlayerData = gameData.PlayerData;
            GameStats = gameData.GameStats;
            CustomRappers = gameData.CustomRappers.ToList();
            Eagles = gameData.Eagles.ToList();
            ShowedTutorials = gameData.ShowedTutorials.ToHashSet();
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

            var gameData = new GameData
            {
                PlayerData = PlayerData,
                GameStats = GameStats,
                CustomRappers = CustomRappers.ToArray(),
                Eagles = Eagles.Take(20).ToArray(),
                ShowedTutorials = ShowedTutorials.ToArray()
            };

            DataManager.Save(gameDataKey, gameData);
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