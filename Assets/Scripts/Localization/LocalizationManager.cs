using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Core;
using UnityEngine;
using UnityEngine.Networking;
using Utils;
using EventType = Core.EventType;

namespace Localization
{
    /// <summary>
    /// Реализация менеджера локализации
    /// Все файлы локализации должны лежать в "Assests/StreamingAssets" в формате json
    /// </summary>
    public class LocalizationManager : Singleton<LocalizationManager> 
    {
        public static readonly SystemLanguage[] AvailableLanguages = {
            SystemLanguage.English,
            SystemLanguage.Russian
        };

        [SerializeField] private LocalizationData _data;

        public bool IsReady { get; private set; }

        protected override void Awake()
        {
            dontDestroy = true;
            base.Awake();
        }

        /// <summary>
        /// Возвращает локализацию переданного ключа
        /// </summary>
        public string Get(string key)
        {
            var item = _data.items.FirstOrDefault(e => e.key == key);
            if (item != null) 
                return item.value;
            
            throw new RapWayException($"Не найдена локализация по ключу [{key}]!");
        }

        /// <summary>
        /// Возвращает ключ локализации по значению
        /// </summary>
        public string GetKey(string value)
        {
            var item = _data.items.First(e => e.value == value);
            return item.key;
        }
        
        /// <summary>
        /// Загружает данные локализации
        /// </summary>
        [SuppressMessage("ReSharper", "JoinDeclarationAndInitializer")]
        public void LoadLocalization(SystemLanguage lang, bool sendEvent = false) 
        {
            if (AvailableLanguages.All(el => el != lang))
                throw new RapWayException($"Язык [{lang}] не поддерживается");

            IsReady = false;
            
            string jsonData;
#if UNITY_ANDROID
            StartCoroutine(ReadDataRoutine(lang, raw => jsonData = raw));
#elif UNITY_IPHONE
            // todo: logic for iphone
#else
            jsonData = File.ReadAllText(WindowsFileName(lang));
#endif
            ParseLocalizationData(jsonData, sendEvent);
        }

        /// <summary>
        /// Корутина считывания данных для андроидов
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private IEnumerator ReadDataRoutine(SystemLanguage lang, Action<string> callback) 
        {
            using (var request = UnityWebRequest.Get(AndroidFileName(lang)))
            {
                yield return request.SendWebRequest();
                callback(request.downloadHandler.text);
            }
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
        /// Выполняет парсинг данных локализации в json
        /// </summary>
        public static string LocalizationDataToJson(LocalizationData data)
            => JsonUtility.ToJson(data);

        /// <summary>
        /// Полный путь к файлу в ОС Windows
        /// </summary>
        public static string WindowsFileName(SystemLanguage lang)
            => Path.Combine(Application.streamingAssetsPath, GetFileName(lang));

        /// <summary>
        /// Полный пусть к файлу в ОС Android
        /// </summary>
        private static string AndroidFileName(SystemLanguage lang)
            => $"jar:file://{Application.dataPath}!/assets/{GetFileName(lang)}";

        /// <summary>
        /// Возвращает имя файла локализации
        /// </summary>
        private static string GetFileName(SystemLanguage lang)
        {
            switch (lang) 
            {
                case SystemLanguage.English:
                    return "en.json";

                // todo: other langs

                default:
                    return "ru.json";
            }
        }
    }
}