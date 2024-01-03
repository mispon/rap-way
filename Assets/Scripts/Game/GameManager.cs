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
        public RapperInfo[] Rappers;
        public RapperInfo[] CustomRappers;
        public LabelInfo[] Labels;
        public LabelInfo[] CustomLabels;
        public LabelInfo PlayerLabel;
        public Eagle[] Eagles;
        public string[] ShowedTutorials;
        public string[] ShowedHints;
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
        [Space]
        public List<RapperInfo> Rappers;
        public List<RapperInfo> CustomRappers;
        [Space]
        public List<LabelInfo> Labels; 
        public List<LabelInfo> CustomLabels;
        public LabelInfo PlayerLabel;
        [Space]
        public List<Eagle> Eagles;
        public HashSet<string> ShowedTutorials;
        public HashSet<string> ShowedHints;

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
                
                Rappers = Array.Empty<RapperInfo>(),
                CustomRappers = Array.Empty<RapperInfo>(),
                
                Labels = Array.Empty<LabelInfo>(),
                CustomLabels = Array.Empty<LabelInfo>(),
                PlayerLabel = null,
                
                Eagles = Array.Empty<Eagle>(),
                ShowedTutorials = Array.Empty<string>(),
                ShowedHints = Array.Empty<string>()
            };
            
            PlayerData = gameData.PlayerData;
            GameStats = gameData.GameStats;
            
            Rappers = gameData.Rappers?.ToList() ?? new List<RapperInfo>(0);
            CustomRappers = gameData.CustomRappers?.ToList() ?? new List<RapperInfo>(0);
            
            Labels = gameData.Labels?.ToList() ?? new List<LabelInfo>(0);
            CustomLabels = gameData.CustomLabels?.ToList() ?? new List<LabelInfo>(0);
            PlayerLabel = gameData.PlayerLabel;
            
            Eagles = gameData.Eagles?.ToList() ?? new List<Eagle>(0);
            ShowedTutorials = gameData.ShowedTutorials?.ToHashSet() ?? new HashSet<string>(0);
            ShowedHints = gameData.ShowedHints?.ToHashSet() ?? new HashSet<string>(0);
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
                
                Rappers = Rappers?.ToArray() ?? Array.Empty<RapperInfo>(),
                CustomRappers = CustomRappers.ToArray(),
                
                Labels = Labels?.ToArray() ?? Array.Empty<LabelInfo>(),
                CustomLabels = CustomLabels?.ToArray(),
                PlayerLabel = PlayerLabel,
                
                Eagles = Eagles.Take(20).ToArray(),
                ShowedTutorials = ShowedTutorials.ToArray(),
                ShowedHints = ShowedHints.ToArray()
            };

            DataManager.Save(gameDataKey, gameData);
        }

        /// <summary>
        /// Checks has been created character or not
        /// </summary>
        public bool HasCharacter()
        {
            return !string.IsNullOrWhiteSpace(PlayerData.Info.NickName);
        }

        /// <summary>
        /// Checks any save exists or not
        /// </summary>
        public bool HasAnySaves()
        {
            return PlayerPrefs.HasKey(gameDataKey);
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