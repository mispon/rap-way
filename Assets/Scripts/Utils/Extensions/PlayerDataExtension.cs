using Models.Player;

namespace Utils.Extensions
{
    /// <summary>
    /// Расширение класса данных игрока
    /// </summary>
    public static class PlayerDataExtension
    {
        /// <summary>
        /// Возвращает презентабельное отображение денег игрока
        /// </summary>
        public static string GetDisplayMoney(this PlayerData data)
        {
            return $"{data.Money} $";
        }
        
        /// <summary>
        /// Возвращает презентабельное отображение фанатов игрока
        /// </summary>
        public static string GetDisplayFans(this PlayerData data)
        {
            return $"{data.Fans}";
        }
    }
}