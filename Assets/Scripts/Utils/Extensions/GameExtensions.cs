using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Utils.Extensions
{
    public static class GameExtensions {
        /// <summary>
        /// Возвращает описание перечисления по значению
        /// </summary>
        public static string GetDescription(this Enum value) 
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var descriptionAttribute = (DescriptionAttribute) Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));
            
            return descriptionAttribute != null 
                ? descriptionAttribute.Description 
                : value.ToString();
        }

        /// <summary>
        /// Возвращает значение перечисления по его описанию
        /// </summary>
        public static T GetFromDescription<T>(string desc)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields()) 
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute) 
                {
                    if (attribute.Description == desc) return (T) field.GetValue(null);
                }
                else 
                {
                    if (field.Name == desc) return (T) field.GetValue(null);
                }
            }

            throw new ArgumentException($"[EnumExtension] Not found item by description - {desc}");
        }

        /// <summary>
        /// Возвращает список описаний всех элементов перечисления  
        /// </summary>
        public static string[] GetDescriptions<T>() where T: Enum
        {
            var values = (T[]) Enum.GetValues(typeof(T));
            return values.Select(e => e.GetDescription()).ToArray();
        }

        /// <summary>
        /// Преобразует коллекцию в хэш-список
        /// </summary>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> values)
        {
            return new HashSet<T>(values);
        }

        /// <summary>
        /// Конвертирует строку в дату
        /// </summary>
        public static DateTime StringToDate(this string value)
        {
            return DateTime.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Конвертирует дату в строку
        /// </summary>
        public static string DateToString(this DateTime value)
        {
            return value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}