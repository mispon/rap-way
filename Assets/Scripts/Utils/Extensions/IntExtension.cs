using System.Globalization;
using UnityEngine;

namespace Utils.Extensions
{
    public static class IntExtension
    {
        /// <summary>
        /// Функция представления денег в удобочитаемом формате 
        /// </summary>
        public static string DisplayMoney(this int value)
        {
            return value.ToString("C0", new CultureInfo("en-US"));
        }
        
        /// <summary>
        /// Функция получения представления целых чисел в формате "##.#k"
        /// </summary>
        public static string DisplayMoneyShort(this int value)
        {
            if (value < 10e3)
                return value.ToString();

            if (value < 10e6)
                return DisplayShort(value, 3);

            if (value < 10e9)
                return DisplayShort(value, 6);

            return DisplayShort(value, 9);
        }

        /// <summary>
        /// Общая функция преобразования, оставляющая только 3 знака до запятой и нужное количестов "k"
        /// </summary>
        private static string DisplayShort(int value, int dividerSize)
        {
            var divider = Mathf.Pow(10, dividerSize);
            var dividedValue = value / divider;
            return $"{dividedValue:###.#}{new string('k', dividerSize / 3)}";
        }
    }
}
