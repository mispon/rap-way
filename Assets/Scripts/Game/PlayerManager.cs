using System.Linq;
using Core.Interfaces;
using Game.UI.GameScreen;
using Localization;
using Models.Player;
using Models.Production;
using UnityEngine;
using Utils;
using Utils.Extensions;

namespace Game
{
    /// <summary>
    /// Логика взаимодействия с данными игрока
    /// </summary>
    public class PlayerManager : Singleton<PlayerManager>, IStarter
    {
        [Header("HUD")]
        [SerializeField] private GameScreenController gameScreen;
        
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
            gameScreen.UpdateHUD(Data);
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
            Data.Fans += fans;
            gameScreen.UpdateHUD(Data);
        }

        /// <summary>
        /// Изменяет количество денег 
        /// </summary>
        public void AddMoney(int money)
        {
            Data.Money += money;
            gameScreen.UpdateHUD(Data);
        }

        /// <summary>
        /// Изменяет количество хайпа 
        /// </summary>
        public void AddHype(int hype)
        {
            Data.Hype += hype;
            gameScreen.UpdateHUD(Data);
        }

        /// <summary>
        /// Выполянет оплату
        /// </summary>
        public bool SpendMoney(int money)
        {
            if (Data.Money < money)
                return false;

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
        /// Возвращает локализированный список тематик, известных игроку  
        /// </summary>
        public static string[] GetPlayersThemes()
        {
            return Data.Themes
                .Select(e => LocalizationManager.Instance.Get(e.GetDescription()))
                .ToArray();
        }
        
        /// <summary>
        /// Возвращает локализированный список стилистик, известных игроку  
        /// </summary>
        public static string[] GetPlayersStyles()
        {
            return Data.Styles
                .Select(e => LocalizationManager.Instance.Get(e.GetDescription()))
                .ToArray();
        }
    }
}