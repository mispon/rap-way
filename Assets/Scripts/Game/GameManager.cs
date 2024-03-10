using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Data;
using Core.Localization;
using Extensions;
using Game.Labels.Desc;
using Game.Player.State.Desc;
using Game.Rappers.Desc;
using Game.Settings;
using Game.Socials.Eagler;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.Game;
using Models.Game;
using Sirenix.OdinInspector;
using UnityEngine;

#pragma warning disable CS0414 // Field is assigned but its value is never used

namespace Game
{
    [Serializable]
    public class GameData
    {
        public PlayerData   PlayerData;
        public GameStats    GameStats;
        public RapperInfo[] Rappers;
        public RapperInfo[] CustomRappers;
        public LabelInfo[]  Labels;
        public LabelInfo[]  CustomLabels;
        public LabelInfo    PlayerLabel;
        public Eagle[]      Eagles;
        public string[]     ShowedHints;
    }
    
    /// <summary>
    /// Логика управления состоянием игры
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        [BoxGroup("Stores URLs"), SerializeField] private string appStoreURL;
        [BoxGroup("Stores URLs"), SerializeField] private string googlePlayURL;
        
        [BoxGroup("Save Key")]  [SerializeField] private string gameDataKey;
        [BoxGroup("Save Key")]  [SerializeField] private string noAdsDataKey;
        [BoxGroup("Save Key")]  [SerializeField] private string donateDataKey;
        
        [BoxGroup("Game Settings")] public GameSettings Settings;
        
        [TabGroup("state", "Player")] public PlayerData PlayerData;
        [TabGroup("state", "Game")]   public GameStats GameStats;
        
        [TabGroup("rappers", "Rappers")]        public List<RapperInfo> Rappers;
        [TabGroup("rappers", "Custom Rappers")] public List<RapperInfo> CustomRappers;
        
        [TabGroup("labels", "Labels")]        public List<LabelInfo> Labels; 
        [TabGroup("labels", "Custom Labels")] public List<LabelInfo> CustomLabels;
        [TabGroup("labels", "Player Label")]  public LabelInfo PlayerLabel;
        
        [BoxGroup("Eagles")]    public List<Eagle> Eagles;
        [BoxGroup("Tutorials")] public HashSet<string> ShowedHints;

        private async void Start()
        {
            LoadApplicationData();
            LocalizationManager.Instance.LoadLocalization(GameStats.Lang, true);
            
            await GetComponent<UnityServicesInitializer>().Initialize();
            
            await Task.Delay(500);
            MsgBroker.Instance.Publish(new GameReadyMessage());
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
        /// Creates new players Save
        /// </summary>
        public PlayerData CreateNewPlayer()
        {
            PlayerData = PlayerData.New;
            Eagles = new List<Eagle>(0);
            
            Rappers = new List<RapperInfo>(0);
            CustomRappers = new List<RapperInfo>(0);

            Labels = new List<LabelInfo>(0);
            CustomLabels = new List<LabelInfo>(0);
            
            LoadDonateBalance();
            
            return PlayerData;
        }

        /// <summary>
        /// Saves no ads setting
        /// </summary>
        public void SaveNoAds()
        {
            PlayerPrefs.SetInt(noAdsDataKey, 1);
        }
        
        /// <summary>
        /// Loads no ads setting 
        /// </summary>
        public bool LoadNoAds()
        {
            return PlayerPrefs.GetInt(noAdsDataKey) == 1;
        }
        
        /// <summary>
        /// Saves donate balance
        /// </summary>
        public void SaveDonateBalance()
        {
            PlayerPrefs.SetInt(donateDataKey, PlayerData.Donate);
        }
        
        /// <summary>
        /// Loads donate balance
        /// </summary>
        private void LoadDonateBalance()
        {
            PlayerData.Donate = PlayerPrefs.GetInt(donateDataKey);
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
                ShowedHints = Array.Empty<string>(),
            };
            
            PlayerData = gameData.PlayerData;
            GameStats = gameData.GameStats;
            
            Rappers = gameData.Rappers?.ToList() ?? new List<RapperInfo>(0);
            CustomRappers = gameData.CustomRappers?.ToList() ?? new List<RapperInfo>(0);
            
            Labels = gameData.Labels?.ToList() ?? new List<LabelInfo>(0);
            CustomLabels = gameData.CustomLabels?.ToList() ?? new List<LabelInfo>(0);
            PlayerLabel = gameData.PlayerLabel;
            
            Eagles = gameData.Eagles?.ToList() ?? new List<Eagle>(0);
            ShowedHints = gameData.ShowedHints?.ToHashSet() ?? new HashSet<string>(0);
            
            LoadDonateBalance();
        }

        /// <summary>
        /// Сохранение данных приложения
        /// </summary>
        public void SaveApplicationData()
        {
            SaveDonateBalance();
            
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

        public bool NeedAskReview()
        {
            return !GameStats.AskedReview && PlayerData.Fans > 0;
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