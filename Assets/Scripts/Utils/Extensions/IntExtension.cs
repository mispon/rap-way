using UnityEngine;

namespace Utils.Extensions
{
    public static class IntExtension
    {
        /// <summary>
        /// Функция получения представления целых чисел в формате "##.#k"
        /// </summary>
        public static string GetDisplay(this int value)
        {
            if (value < 1000)
                return value.ToString();

            if (value < 1000000)
                return value.GetDisplayHelp(3);

            if (value < 1000000000)
                return value.GetDisplayHelp(6);

            return value.GetDisplayHelp(9);
        }

        /// <summary>
        /// Общая функция преобразования, оставляющая только 3 знака до запятой и нужное количестов "k"
        /// </summary>
        private static string GetDisplayHelp(this int value, int dividerSize)
        {
            var divider = Mathf.Pow(10, dividerSize);
            var dividedValue = value / divider;
            return $"{dividedValue:###.#}{new string('k', dividerSize / 3)}";
        }
    }
}
