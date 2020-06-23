using System.Linq;
using Models.Player;
using Models.Production;
using Utils;

namespace Core
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        public static PlayerData PlayerData => GameManager.Instance.PlayerData;

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
        /// Возвращает идентификатор для новой сущности 
        /// </summary>
        public static int GetNextId<T>() where T : Production
        {
            var history = PlayerData.History;
            var id = 0;
            
            if (typeof(T) == typeof(TrackInfo))
                id = history.TrackList.Any() ? history.TrackList.Max(e => e.Id) : 0;

            if (typeof(T) == typeof(ClipInfo))
                id = history.TrackList.Any() ? history.ClipList.Max(e => e.Id) : 0;

            if (typeof(T) == typeof(AlbumInfo))
                id = history.TrackList.Any() ? history.AlbumList.Max(e => e.Id) : 0;

            if (typeof(T) == typeof(ConcertInfo))
                id = history.TrackList.Any() ? history.ConcertList.Max(e => e.Id) : 0;

            return id + 1;
        }
    }
}