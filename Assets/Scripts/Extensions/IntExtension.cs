namespace Extensions
{
    public static class IntExtension
    {
        /// <summary>
        /// Функция представления величины в удобочитаемом формате
        /// </summary>
        public static string GetDisplay(this int value)
        {
            return $"{value:N0}";
        }

        /// <summary>
        /// Функция представления денег в удобочитаемом формате
        /// </summary>
        public static string GetMoney(this int value)
        {
            return $"{value:N0}$";
        }
    }
}