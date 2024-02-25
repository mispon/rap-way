using Core;
using Core.OrderedStarter;
using Enums;
using Game.Player.State.Desc;
using Models.Trends;

namespace Game.Player
{
    /// <summary>
    /// Логика взаимодействия с данными игрока
    /// </summary>
    /// TODO: избавиться от этого менеджера?
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