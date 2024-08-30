using System;
using System.Collections;
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
using Game.SocialNetworks.Eagler;
using Game.SocialNetworks.Email;
using Game.SocialNetworks.News;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.Game;
using Models.Game;
using UniRx;
using UnityEngine;

#pragma warning disable CS0414 // Field is assigned but its value is never used

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
        public Email[] Emails;
        public News[] News;
        public string[] ShowedHints;
    }

    /// <summary>
    /// Логика управления состоянием игры
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        [Header("Stores URLs"), SerializeField] private string appStoreURL;
        [Header("Stores URLs"), SerializeField] private string googlePlayURL;

        [Space]
        [Header("Save Key")][SerializeField] private string gameDataKey;
        [Header("Save Key")][SerializeField] private string noAdsDataKey;
        [Header("Save Key")][SerializeField] private string donateDataKey;

        [Space]
        [Header("Game Settings")] public GameSettings Settings;
        [Header("Player")] public PlayerData PlayerData;
        [Header("Game")] public GameStats GameStats;

        [Space]
        [Header("Rappers")] public List<RapperInfo> Rappers;
        [Header("Custom Rappers")] public List<RapperInfo> CustomRappers;

        [Space]
        [Header("Labels")] public List<LabelInfo> Labels;
        [Header("Custom Labels")] public List<LabelInfo> CustomLabels;
        [Header("Player Label")] public LabelInfo PlayerLabel;

        [Space]
        [Header("Eagles")] public List<Eagle> Eagles;
        [Header("Emails")] public List<Email> Emails;
        [Header("News")] public List<News> News;
        [Header("Tutorials")] public HashSet<string> ShowedHints;

        public bool Ready;

        private readonly CompositeDisposable _disposables = new();

        private void Start()
        {
            LoadApplicationData();
            LocalizationManager.Instance.LoadLocalization(GameStats.Lang, true);

            RegisterHandlers();
            StartCoroutine(SetReady());
        }

        private IEnumerator SetReady()
        {
            yield return new WaitForSeconds(1);

            Ready = true;
            MsgBroker.Instance.Publish(new GameReadyMessage());
        }

        private void RegisterHandlers()
        {
            MsgBroker.Instance
                .Receive<LangChangedMessage>()
                .Subscribe(e =>
                {
                    GameStats.Lang = e.Lang;
                    SaveApplicationData();
                })
                .AddTo(_disposables);
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
        /// Creates new players save
        /// </summary>
        public PlayerData CreateNewPlayer()
        {
            PlayerData = PlayerData.New;
            Eagles = new List<Eagle>(0);
            Emails = new List<Email>(0);
            News = new List<News>(0);

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
                Emails = Array.Empty<Email>(),
                News = Array.Empty<News>(),

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
            Emails = gameData.Emails?.ToList() ?? new List<Email>(0);
            News = gameData.News?.ToList() ?? new List<News>(0);

            ShowedHints = gameData.ShowedHints?.ToHashSet() ?? new HashSet<string>(0);

            LoadDonateBalance();
        }

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
                Emails = Emails.Take(50).ToArray(),
                News = News.Take(50).ToArray(),

                ShowedHints = ShowedHints.ToArray()
            };

            DataManager.Save(gameDataKey, gameData);
        }

        /// <summary>
        /// Checks if character has been created or not
        /// </summary>
        public bool HasCharacter()
        {
            return !string.IsNullOrWhiteSpace(PlayerData.Info.NickName);
        }

        /// <summary>
        /// Checks if lang already selected
        /// </summary>
        public bool IsLangSelected()
        {
            return GameStats.LangSelected;
        }

        /// <summary>
        /// Checks if game available to show review asking page
        /// </summary>
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
                SoundManager.Instance.PauseMusic();
            }
            else
            {
                SoundManager.Instance.UnpauseMusic();
            }
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus)
            {
                SoundManager.Instance.UnpauseMusic();
            }
            else
            {
                SoundManager.Instance.PauseMusic();
            }
        }

        private void OnDestroy()
        {
            _disposables.Clear();
        }
    }
}