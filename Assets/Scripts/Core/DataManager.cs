using Models.Game;
using Models.Player;
using Utils;

namespace Core
{
    /// <summary>
    /// Управление сохранением и загрузкой игровых данных
    /// </summary>
    public class DataManager: Singleton<DataManager>
    {
        public PlayerData PlayerData;
        public GameStats GameStats;

        public void Save()
        {
            
        }

        public void Load()
        {
            
        }
    }
}