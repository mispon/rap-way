using System;
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
        
        [Header("Компоненты")]
        [SerializeField] private DataManager dataManager;
        
        public PlayerData PlayerData { get; private set; }
        public GameStats GameStats { get; private set; }

        private void Start()
        {
            // todo: включить загрузочный экран

            LoadApplicationData();
            TimeManager.Instance.Setup(GameStats.Now);
            
            // todo: выключить загрузочный экран
        }

        /// <summary>
        /// Выдает награду за завершение основного действия
        /// </summary>
        public void GiveReward(int fans, int money)
        {
            AddFans(fans);
            AddMoney(money);
        }
        
        /// <summary>
        /// Изменяет количество фанатов 
        /// </summary>
        public void AddFans(int fans)
        {
            PlayerData.Data.Fans += fans;
        }

        /// <summary>
        /// Изменяет количество денег 
        /// </summary>
        public void AddMoney(int money)
        {
            PlayerData.Data.Money += money;
        }

        /// <summary>
        /// Изменяет количество хайпа 
        /// </summary>
        public void AddHype(int hype)
        {
            PlayerData.Data.Hype += hype;
        }

        /// <summary>
        /// Загрузка данных приложения
        /// </summary>
        private void LoadApplicationData()
        {
            PlayerData = dataManager.Load<PlayerData>(playersDataKey) ?? PlayerData.New;
            GameStats = dataManager.Load<GameStats>(gameDataKey) ?? GameStats.New;
        }

        /// <summary>
        /// Сохранение данных приложения
        /// </summary>
        private void SaveApplicationData()
        {
            dataManager.Save(PlayerData, playersDataKey);
            dataManager.Save(GameStats, gameDataKey);
        }

        private void OnDisable()
        {
            SaveApplicationData();
        }
    }
}