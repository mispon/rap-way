using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Data;
using Core.Localization;
using Extensions;
using Game.Labels.Desc;
using Game.Player.Character;
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
    public class GameManager : Singleton<GameManager>
    {
        [Header("Stores URLs")]
        [SerializeField]
        private string appStoreURL;
        [Header("Stores URLs")]
        [SerializeField]
        private string googlePlayURL;

        [Space]
        [Header("Game Settings")]
        public GameSettings Settings;

        [Space]
        [Header("Game")]
        public GameStats GameStats;
        [Header("Player")]
        public PlayerData PlayerData;

        [Space]
        [Header("Rappers")]
        public List<RapperInfo> Rappers;
        [Header("Custom Rappers")]
        public List<RapperInfo> CustomRappers;

        [Space]
        [Header("Labels")]
        public List<LabelInfo> Labels;
        [Header("Custom Labels")]
        public List<LabelInfo> CustomLabels;
        [Header("Player Label")]
        public LabelInfo PlayerLabel;

        [Space]
        [Header("Eagles")]
        public List<Eagle> Eagles;
        [Header("Emails")]
        public List<Email> Emails;
        [Header("News")]
        public List<News> News;

        [Space]
        [Header("Tutorials")]
        public HashSet<string> ShowedHints;

        public bool Ready;

        private readonly CompositeDisposable _disposables = new();

        private void Start()
        {
            LoadApplicationData();
            LocalizationManager.Instance.LoadLocalization(GameStats.Lang, true);

            Character.Instance.Load();
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
        ///     Creates new players save
        /// </summary>
        public PlayerData CreateNewPlayer()
        {
            PlayerData = PlayerData.New;
            Eagles     = new List<Eagle>(0);
            Emails     = new List<Email>(0);
            News       = new List<News>(0);

            Rappers       = new List<RapperInfo>(0);
            CustomRappers = new List<RapperInfo>(0);

            Labels       = new List<LabelInfo>(0);
            CustomLabels = new List<LabelInfo>(0);

            LoadDonateBalance();

            return PlayerData;
        }

        private void LoadApplicationData()
        {
            GameStats  = DataManager.Load<GameStats>(DataKeys.GameStats) ?? GameStats.New;
            PlayerData = DataManager.Load<PlayerData>(DataKeys.PlayerData) ?? PlayerData.New;

            Rappers       = DataManager.LoadArray<RapperInfo>(DataKeys.Rappers)?.ToList() ?? new List<RapperInfo>(0);
            CustomRappers = DataManager.LoadArray<RapperInfo>(DataKeys.CustomRappers)?.ToList() ?? new List<RapperInfo>(0);

            Labels       = DataManager.LoadArray<LabelInfo>(DataKeys.Labels)?.ToList() ?? new List<LabelInfo>(0);
            CustomLabels = DataManager.LoadArray<LabelInfo>(DataKeys.CustomLabels)?.ToList() ?? new List<LabelInfo>(0);
            PlayerLabel  = DataManager.Load<LabelInfo>(DataKeys.PlayerLabel);

            Eagles = DataManager.LoadArray<Eagle>(DataKeys.Eagles)?.ToList() ?? new List<Eagle>(0);
            Emails = DataManager.LoadArray<Email>(DataKeys.Emails)?.ToList() ?? new List<Email>(0);
            News   = DataManager.LoadArray<News>(DataKeys.News)?.ToList() ?? new List<News>(0);

            ShowedHints = DataManager.LoadArray<string>(DataKeys.ShowedHints)?.ToHashSet() ?? new HashSet<string>(0);

            LoadDonateBalance();
        }

        public void SaveApplicationData()
        {
            if (TimeManager.Instance != null)
            {
                GameStats.Now = TimeManager.Instance.Now.DateToString();
            }

            DataManager.Save(DataKeys.GameStats, GameStats);
            DataManager.Save(DataKeys.PlayerData, PlayerData);

            DataManager.SaveArray(DataKeys.Rappers, Rappers);
            DataManager.SaveArray(DataKeys.CustomRappers, CustomRappers);

            DataManager.SaveArray(DataKeys.Labels, Labels);
            DataManager.SaveArray(DataKeys.CustomLabels, CustomLabels);
            DataManager.Save(DataKeys.PlayerLabel, PlayerLabel);

            DataManager.SaveArray(DataKeys.Eagles, Eagles.Take(30));
            DataManager.SaveArray(DataKeys.Emails, Emails.Take(50));
            DataManager.SaveArray(DataKeys.News, News.Take(50));

            DataManager.SaveArray(DataKeys.ShowedHints, ShowedHints);

            SaveDonateBalance();
        }

        /// <summary>
        ///     Saves no ads setting
        /// </summary>
        public void SaveNoAds()
        {
            PlayerPrefs.SetInt(DataKeys.NoAds, 1);
        }

        /// <summary>
        ///     Loads no ads setting
        /// </summary>
        public bool LoadNoAds()
        {
            return PlayerPrefs.GetInt(DataKeys.NoAds) == 1 ||
                   PlayerPrefs.GetInt(DataKeys.NoAdsOld) == 1;
        }

        /// <summary>
        ///     Saves donate balance
        /// </summary>
        private void SaveDonateBalance()
        {
            PlayerPrefs.SetInt(DataKeys.DonateBalance, PlayerData.Donate);
        }

        /// <summary>
        ///     Loads donate balance
        /// </summary>
        private void LoadDonateBalance()
        {
            PlayerData.Donate = PlayerPrefs.GetInt(DataKeys.DonateBalance);
        }

        /// <summary>
        ///     Checks if character has been created or not
        /// </summary>
        public bool HasCharacter()
        {
            return !string.IsNullOrWhiteSpace(PlayerData.Info.NickName);
        }

        /// <summary>
        ///     Checks if lang already selected
        /// </summary>
        public bool IsLangSelected()
        {
            return GameStats.LangSelected;
        }

        /// <summary>
        ///     Checks if game available to show review asking page
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
            } else
            {
                SoundManager.Instance.UnpauseMusic();
            }
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus)
            {
                SoundManager.Instance.UnpauseMusic();
            } else
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