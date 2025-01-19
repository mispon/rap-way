using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Data
{
    [Serializable]
    public class ArrayValue<T>
    {
        public T[] Arr;
    }

    public static class DataManager
    {
        public static void Save<T>(string saveKey, T data) where T : class
        {
            var jsonData = JsonUtility.ToJson(data);

            if (string.IsNullOrEmpty(jsonData))
            {
                Debug.LogWarning($"Failed to marshal data by key: {saveKey}, data: {data}");
                return;
            }

            PlayerPrefs.SetString(saveKey, jsonData);
        }

        public static void SaveArray<T>(string saveKey, IEnumerable<T> items) where T : class
        {
            var arrayValue = new ArrayValue<T>
            {
                Arr = items?.ToArray() ?? Array.Empty<T>()
            };

            Save(saveKey, arrayValue);
        }

        public static T Load<T>(string saveKey) where T : class
        {
            T result = null;

            if (PlayerPrefs.HasKey(saveKey))
            {
                var jsonData = PlayerPrefs.GetString(saveKey);
                result = JsonUtility.FromJson<T>(jsonData);
            }

            return result;
        }

        public static T[] LoadArray<T>(string saveKey) where T : class
        {
            var arrayValue = Load<ArrayValue<T>>(saveKey);
            return arrayValue?.Arr ?? Array.Empty<T>();
        }
    }
}