using System.Linq;
using Core;
using Core.OrderedStarter;
using Enums;
using Models.Player;
using Models.Production;
using Models.Trends;

namespace Game.Player
{
    /// <summary>
    /// Логика взаимодействия с данными игрока
    /// </summary>
    /// TODO: Перейти на MessageBroker и избавиться от этого менеджера
    public class PlayerManager : Singleton<PlayerManager>, IStarter
    {
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
        }

        /// <summary>
        /// Возвращает идентификатор для новой сущности
        /// </summary>
        public static int GetNextProductionId<T>() where T : ProductionBase
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
        /// Устанавливает время отдыха указанному тиммейту
        /// </summary>
        public static void SetTeammateCooldown(TeammateType type, int cooldown)
        {
            var teammate = Data.Team.TeammatesArray.First(e => e.Type == type);
            teammate.Cooldown = cooldown;
        }

        /// <summary>
        /// Обновляет информацию о трендах
        /// </summary>
        public static void UpdateTrends(Styles style, Themes theme)
        {
            Data.LastKnownTrends ??= Trends.New;

            Data.LastKnownTrends.Style = style;
            Data.LastKnownTrends.Theme = theme;
        }
    }
}