using System;
using System.Collections;
using System.IO;
using System.Linq;
using MessageBroker;
using MessageBroker.Messages.Game;
using UnityEngine;
using UnityEngine.Networking;

namespace Core.Localization
{
    public enum GameLang
    {
        RU,
        EN,
        DE,
        FR,
        IT,
        ES,
        PT
    }

    /// <summary>
    /// Реализация менеджера локализации
    /// Все файлы локализации должны лежать в "Assests/StreamingAssets" в формате json
    /// </summary>
    public class LocalizationManager : Singleton<LocalizationManager>
    {
        [SerializeField] private LocalizationData _data;
        private LocalizationData _enBackup;

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

            item = _enBackup.items.FirstOrDefault(e => e.key == key);
            if (item != null)
                return item.value;

            throw new RapWayException($"Not found localization by key [{key}]!");
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
            return args != null && args.Length > 0
                ? string.Format(Get(key), args)
                : Get(key);
        }

        /// <summary>
        /// Загружает данные локализации
        /// </summary>
        public void LoadLocalization(GameLang lang, bool sendEvent = false)
        {
            StartCoroutine(LoadLocalizationAsync(lang, sendEvent));
            StartCoroutine(LoadBackupLocalizationAsync());
        }

        /// <summary>
        /// Корутина загрузки данных локализации
        /// </summary>
        private IEnumerator LoadLocalizationAsync(GameLang lang, bool sendEvent = false)
        {
            string path = Path.Combine(Application.streamingAssetsPath, GetFileName(lang));

            string jsonData = "";
#if UNITY_ANDROID
            yield return LoadAndroidLocalization(path, data => jsonData = data);
#elif UNITY_IPHONE
            // TODO
#else
            jsonData = File.ReadAllText(path);
#endif

            ParseLocalizationData(jsonData);
            if (sendEvent)
            {
                MsgBroker.Instance.Publish(new LangChangedMessage { Lang = lang });
            }

            yield return null;
        }

        private IEnumerator LoadBackupLocalizationAsync()
        {
            string path = Path.Combine(Application.streamingAssetsPath, GetFileName(GameLang.EN));

            string jsonData = "";
#if UNITY_ANDROID
            yield return LoadAndroidLocalization(path, data => jsonData = data);
#elif UNITY_IPHONE
            // TODO
#else
            jsonData = File.ReadAllText(path);
#endif
            ParseBackupLocalizationData(jsonData);

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
        private void ParseLocalizationData(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
                throw new RapWayException("Не найден файл локализации!");

            _data = JsonUtility.FromJson<LocalizationData>(jsonData);
        }

        /// <summary>
        /// Выполняет парсинг файла с локализацией
        /// </summary>
        private void ParseBackupLocalizationData(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
                throw new RapWayException("Не найден файл локализации!");

            _enBackup = JsonUtility.FromJson<LocalizationData>(jsonData);
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
            return lang switch
            {
                GameLang.RU => "ru.json",
                GameLang.EN => "en.json",
                GameLang.DE => "de.json",
                GameLang.FR => "fr.json",
                GameLang.IT => "it.json",
                GameLang.ES => "es.json",
                GameLang.PT => "pt.json",
                _ => throw new RapWayException($"Неизвестное значение языка: {lang}!")
            };
        }
    }
}