using System.Linq;
using Game.UI.GameScreen;
using Localization;
using Models.Player;
using Models.Production;
using Utils;

namespace Game
{
    /// <summary>
    /// Логика взаимодействия с данными игрока
    /// </summary>
    public class PlayerManager : Singleton<PlayerManager>
    {
        public static PlayerData PlayerData => GameManager.Instance.PlayerData;
        
        private GameScreenController _gameScreen;

        /// <summary>
        /// Устанавливает ссылку на интерфейс игрока
        /// </summary>
        public void SetHUD(GameScreenController gameScreen)
        {
            _gameScreen = gameScreen;
            _gameScreen.UpdateHUD(PlayerData);
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
            _gameScreen.UpdateHUD(PlayerData);
        }

        /// <summary>
        /// Изменяет количество денег 
        /// </summary>
        public void AddMoney(int money)
        {
            PlayerData.Data.Money += money;
            _gameScreen.UpdateHUD(PlayerData);
        }

        /// <summary>
        /// Изменяет количество хайпа 
        /// </summary>
        public void AddHype(int hype)
        {
            PlayerData.Data.Hype += hype;
            _gameScreen.UpdateHUD(PlayerData);
        }

        /// <summary>
        /// Выполянет оплату
        /// </summary>
        public bool SpendMoney(int money)
        {
            if (PlayerData.Data.Money < money)
                return false;

            AddMoney(-money);
            return true;
        }

        /// <summary>
        /// Возвращает идентификатор для новой сущности 
        /// </summary>
        public static int GetNextProductionId<T>() where T : Production
        {
            var history = PlayerData.History;
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
            return PlayerData.Themes
                .Select(e => LocalizationManager.Instance.Get(e.GetDescription()))
                .ToArray();
        }
        
        /// <summary>
        /// Возвращает локализированный список стилистик, известных игроку  
        /// </summary>
        public static string[] GetPlayersStyles()
        {
            return PlayerData.Styles
                .Select(e => LocalizationManager.Instance.Get(e.GetDescription()))
                .ToArray();
        }
        
    }
}