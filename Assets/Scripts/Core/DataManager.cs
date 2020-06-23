using UnityEngine;
using Utils;

namespace Core
{
    /// <summary>
    /// Управление сохранением и загрузкой игровых данных
    /// </summary>
    public static class DataManager
    {
        /// <summary>
        /// Выполняет сохранение игровых данных 
        /// </summary>
        public static void Save<T>(T data, string saveKey) where T : class
        {
            var jsonData = JsonUtility.ToJson(data);
            
            if (string.IsNullOrEmpty(jsonData)) 
                throw new RapWayException($"Отсутствуют данные типа {typeof(T)} по ключу {saveKey}");
            
            PlayerPrefs.SetString(saveKey, jsonData);
        }

        /// <summary>
        /// Выполняет загрузку игровых данных 
        /// </summary>
        public static T Load<T>(string saveKey) where T : class
        {
            T result = null;
            
            if (PlayerPrefs.HasKey(saveKey)) {
                var jsonData = PlayerPrefs.GetString(saveKey);
                result = JsonUtility.FromJson<T>(jsonData);
            }

            return result;
        }

        /// <summary>
        /// Удаляет игровые данные
        /// </summary>
        public static void Clear(string saveKey)
        {
            PlayerPrefs.DeleteKey(saveKey);
        }
    }
}