using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Core;
using UnityEngine;
using Utils;
using EventType = Core.EventType;

namespace Localization
{
    /// <summary>
    /// Реализация менеджера локализации
    /// NOTE: All localization files must be in "Assests/StreamingAssets" folder in .json format
    /// </summary>
    public class LocalizationManager : Singleton<LocalizationManager> 
    {
        /// <summary>
        /// Хранилище данных локализации
        /// </summary>
        [SerializeField] private LocalizationData _data;

        public bool IsReady { get; private set; }

        /// <summary>
        /// Возвращает локализацию переданного ключа
        /// </summary>
        public string Get(string key)
        {
            var item = _data.items.FirstOrDefault(e => e.key == key);
            if (item != null) 
                return item.value;
            
            throw new RapWayException($"Не найдена локализация по ключу {key}!");
        }

        /// <summary>
        /// Возвращает ключ локализации по значению
        /// </summary>
        public string GetKey(string value)
        {
            var item = _data.items.First(e => e.value == value);
            return item.value;
        }

        /// <summary>
        /// Загружает данные локализации
        /// </summary>
        [SuppressMessage("ReSharper", "JoinDeclarationAndInitializer")]
        public void LoadLocalization(int value, bool sendEvent = false) 
        {
            IsReady = false;
            
            string jsonData;
#if UNITY_ANDROID
            StartCoroutine(ReadDataRoutine(value, raw => jsonData = raw));
#elif UNITY_IPHONE
            // todo: logic for iphone
#else
            var filePath = Path.Combine(Application.streamingAssetsPath, GetFileName(value));
            jsonData = File.ReadAllText(filePath);
#endif
            ParseLocalizationData(jsonData, sendEvent);
        }

        /// <summary>
        /// Routine read localization data for android
        /// </summary>
        private IEnumerator ReadDataRoutine(int value, Action<string> callback) 
        {
            var filePath = "jar:file://" + Application.dataPath + "!/assets/" + GetFileName(value);
            var www = new WWW(filePath);
            yield return www;
            callback(www.text);
        }

        /// <summary>
        /// Выполняет парсинг файла с локализацией
        /// </summary>
        private void ParseLocalizationData(string jsonData, bool sendEvent)
        {
            if (string.IsNullOrEmpty(jsonData)) 
                throw new RapWayException("Не найден файл локализации!");
                
            _data = JsonUtility.FromJson<LocalizationData>(jsonData);
            
            if (sendEvent) 
                EventManager.RaiseEvent(EventType.LangChanged);

            IsReady = true;
        }

        /// <summary>
        /// Возвращает имя файла локализации
        /// </summary>
        private static string GetFileName(int value)
        {
            switch (value) 
            {
                case 1:
                    return "en.json";
                
                // todo: other langs
                
                default:
                    return "ru.json";
            }
        }
    }
}