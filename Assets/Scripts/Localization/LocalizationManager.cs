using System;
using System.Collections;
using System.IO;
using System.Linq;
using Core;
using UnityEngine;
using UnityEngine.Networking;
using Utils;
using EventType = Core.EventType;

namespace Localization
{
    public enum GameLang
    {
        RU,
        EN
    }

    /// <summary>
    /// Реализация менеджера локализации
    /// Все файлы локализации должны лежать в "Assests/StreamingAssets" в формате json
    /// </summary>
    public class LocalizationManager : Singleton<LocalizationManager>
    {
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
        /// Возвращает локализованную строку с подставленными параметрами
        /// </summary>
        public string GetFormat(string key, params object[] args)
        {
            return string.Format(Get(key), args);
        }

        /// <summary>
        /// Загружает данные локализации
        /// </summary>
        public void LoadLocalization(GameLang lang, bool sendEvent = false)
        {
            StartCoroutine(LoadLocalizationAsync(lang, sendEvent));
        }

        /// <summary>
        /// Корутина загрузки данных локализации
        /// </summary>
        private IEnumerator LoadLocalizationAsync(GameLang lang, bool sendEvent = false)
        {
            IsReady = false;

            string jsonData = "";
            string path = Path.Combine(Application.streamingAssetsPath, GetFileName(lang));
#if UNITY_ANDROID
            yield return LoadAndroidLocalization(path, data => jsonData = data);
#elif UNITY_IPHONE
            // TODO
#else
            jsonData = File.ReadAllText(path);
#endif
            ParseLocalizationData(jsonData, sendEvent);

            IsReady = true;
            yield return null;
        }

        /// <summary>
        /// Загружает файл локализации на андроиде
        /// </summary>
        private static IEnumerator LoadAndroidLocalization(string path, Action<string> callback)
        {
            var request = UnityWebRequest.Get(path);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ProtocolError)
                Debug.LogError(request.error);
            else
                callback.Invoke(request.downloadHandler.text);
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
        /// Возвращает путь к файлу локализации
        /// </summary>
        public static string GetLocalizationPath(GameLang lang)
            => Path.Combine(Application.streamingAssetsPath, GetFileName(lang));

        /// <summary>
        /// Возвращает имя файла локализации
        /// </summary>
        private static string GetFileName(GameLang lang)
        {
            switch (lang)
            {
                case GameLang.RU:
                    return "ru.json";
                case GameLang.EN:
                    return "en.json";
                default:
                    throw new RapWayException($"Неизвестное значение языка: {lang}!");
            }
        }
    }
}