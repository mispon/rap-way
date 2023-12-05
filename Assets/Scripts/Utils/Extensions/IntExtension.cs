using System;

namespace Utils.Extensions
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

        /// <summary>
        /// Returns shortened int representation
        /// Eg. 1M or 2.5T
        /// </summary>
        public static string GetShortened(this int value)
        {
            const int m = 1_000_000;
            if (value >= m)
            {
                float v = 1f * value / m;
                return $"{v:F1}M";
            }

            const int t = 1_000;
            if (value >= t)
            {
                float v = 1f * value / t;
                return $"{v:F1}T";
            } 
            
            return $"{value}";
        }
    }
}