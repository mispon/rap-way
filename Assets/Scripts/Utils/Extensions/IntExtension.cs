using UnityEngine;

namespace Utils.Extensions
{
    public static class IntExtension
    {
        /// <summary>
        /// Функция получения представления целых чисел в формате "##.#k"
        /// </summary>
        public static string GetDescription(this int value)
        {
            if (value < 1000)
                return value.ToString();

            if (value < 1000000)
                return value.GetDescriptionHelp(3);

            if (value < 1000000000)
                return value.GetDescriptionHelp(6);

            return value.GetDescriptionHelp(9);
        }

        /// <summary>
        /// Общая функция преобразования, оставляющая только 3 знака до запятой и нужное количестов "k"
        /// </summary>
        private static string GetDescriptionHelp(this int value, int dividerSize)
        {
            var divider = Mathf.Pow(10, dividerSize);
            var dividedValue = value / divider;
            return $"{dividedValue:###.#}{new string('k', dividerSize / 3)}";
        }
    }
}
