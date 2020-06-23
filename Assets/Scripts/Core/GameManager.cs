using Localization;
using Models.Game;
using Models.Player;
using UnityEngine;
using Utils;

namespace Core
{
    /// <summary>
    /// Логика управления состоянием игры
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        [Header("Ключи сохранения данных")]
        [SerializeField] private string playersDataKey;
        [SerializeField] private string gameDataKey;

        public PlayerData PlayerData { get; private set; }
        public GameStats GameStats { get; private set; }

        private void Start()
        {
            // todo: включить загрузочный экран

            LoadApplicationData();
            LocalizationManager.Instance.LoadLocalization(GameStats.Lang, true);
            TimeManager.Instance.Setup(GameStats.Now);
            
            // todo: выключить загрузочный экран
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
        private void LoadApplicationData()
        {
            PlayerData = DataManager.Load<PlayerData>(playersDataKey) ?? PlayerData.New;
            GameStats = DataManager.Load<GameStats>(gameDataKey) ?? GameStats.New;
        }

        /// <summary>
        /// Сохранение данных приложения
        /// </summary>
        private void SaveApplicationData()
        {
            DataManager.Save(PlayerData, playersDataKey);
            DataManager.Save(GameStats, gameDataKey);
        }

        private void OnDisable()
        {
            // SaveApplicationData();
        }
    }
}