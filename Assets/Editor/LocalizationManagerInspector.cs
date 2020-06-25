using UnityEngine;
using UnityEditor;
using Localization;
using System.IO;
using Utils;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(LocalizationManager))]
public class LocalizationManagerInspector: UnityEditor.Editor
{
    private static float defaultLabelWidth = 100;
    private const float toggleLabelWidth = 30;
    private const float textAreaLabelWidth = 50;
    
    //++Settings
    private static bool showSettings = true;
    private static SystemLanguage _previousLanguage = SystemLanguage.Russian;
    private static SystemLanguage _language = SystemLanguage.Russian;
    private static SystemLanguage language
    {
        get => _language;
        set
        {
            _language = LocalizationManager.AvailableLanguages.Any(al => al == value) ?
                value :
                _previousLanguage;

            if(_previousLanguage != _language)
            {
                if (autoSave || EditorUtility.DisplayDialog("При смене языка:", $"[{_previousLanguage}]: Сохранить изменения?", "Да", "Нет"))
                    SaveLocalization(_previousLanguage, in items);

                if (autoLoad || EditorUtility.DisplayDialog("При смене языка:", $"[{_language}]: Загрузить локализацию?", "Да", "Нет"))
                    LoadLocalization(_language, ref items, true);

                newItems.Clear();
            }
            _previousLanguage = _language;
        }
    }
    private static bool autoLoad = true;
    private static bool autoSave = true;
    //--Settings

    //++Data
    private static bool showData = true;
    private static List<LocalizationItem> items = new List<LocalizationItem>();
    private static List<bool> enableEditData = new List<bool>();
    //--Data

    //++New data
    private static bool showNewData = true;
    private static List<LocalizationItem> newItems = new List<LocalizationItem>();
    //--New data

    //++ Merge data
    private static bool showMerge = true;
    private static SystemLanguage _mergeLanguage = SystemLanguage.English;
    private static SystemLanguage mergeLanguage
    {
        get => _mergeLanguage;
        set
        {
            _mergeLanguage = LocalizationManager.AvailableLanguages.Any(al => al == value) ?
                value :
                SystemLanguage.English;
        }
    }
    //--Merge data

    private void OnEnable()
    {
        if (items.Count != 0 || Application.isPlaying)
            return;

        if (autoLoad)
        {
            LoadLocalization(language, ref items, true);
            return;
        }

        if (autoSave || EditorUtility.DisplayDialog("При активации:", $"[{language}]: Сохранить в файл новые данные по языку?", "Да", "Нет"))
            SaveLocalization(language, in items);
    }

    private void OnDisable()
    {
        if (Application.isPlaying)
            return;
        
        if (autoSave || EditorUtility.DisplayDialog("При закрыти:", $"[{language}]: Сохранить изменения по языку?", "Да", "Нет"))
            SaveLocalization(language, in items);
    }

    public override void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            base.OnInspectorGUI();
            return;
        }

        defaultLabelWidth = EditorGUIUtility.labelWidth;
        
        DrawSettings();
        EditorGUILayout.Space();

        EditorGUIUtility.labelWidth = textAreaLabelWidth;

        DrawExistedData();
        EditorGUILayout.Space();

        DrawNewData();
        EditorGUILayout.Space();

        DrawMergeData();
    }
    

    /// <summary>
    /// Отрисовка кнопок загрузки/сохранения данных
    /// </summary>
    private void DrawSettings()
    {
        showSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showSettings, showSettings ? $"[{language}]: Скрыть настройки" : $"[{language}]: Отобразить настройки");
        if (showSettings)
        {
            language = (SystemLanguage)EditorGUILayout.EnumPopup("Язык локализации", language);
            
            EditorGUILayout.BeginHorizontal();
            autoLoad = EditorGUILayout.ToggleLeft("Автозагрузка", autoLoad);
            autoSave = EditorGUILayout.ToggleLeft("Автосохранение", autoSave);
            EditorGUILayout.EndHorizontal();

            if (autoLoad && autoSave)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }
                

            EditorGUILayout.BeginHorizontal();
            if (!autoLoad)
            {
                if (GUILayout.Button("Загрузить"))
                {
                    LoadLocalization(language, ref items, true);
                    Repaint();
                }
            }
            if (!autoSave)
            {
                if (GUILayout.Button("Сохранить"))
                {
                    SaveLocalization(language, in items);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    /// <summary>
    /// Отображает данные, занесенные в data
    /// </summary>
    private void DrawExistedData()
    {
        showData = EditorGUILayout.BeginFoldoutHeaderGroup(showData, showData ? $"[{language}]: Скрыть данные" : $"[{language}]: Отобразить данные");
        if (!showData)
        {
            EditorGUILayout.EndFoldoutHeaderGroup();
            return;
        }

        for(int i = 0 ; i < items.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                items.RemoveAt(i);
                enableEditData.RemoveAt(i);
                if (autoSave)
                    SaveLocalization(language, in items);
                Repaint();
            }

            if(i >= items.Count)
                break;
                
            //Edit toggle
            EditorGUIUtility.labelWidth = toggleLabelWidth;
            enableEditData[i] = EditorGUILayout.Toggle("Edit:", enableEditData[i], GUILayout.Width(60));

            //Item data
            EditorGUIUtility.labelWidth = textAreaLabelWidth;
            GUI.enabled = enableEditData[i];
            items[i].key = EditorGUILayout.TextField("Key:", items[i].key);
            items[i].value = EditorGUILayout.TextField("Value:", items[i].value);
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    /// <summary>
    /// Отображение данных для добавления
    /// </summary>
    private void DrawNewData()
    {
        showNewData = EditorGUILayout.BeginFoldoutHeaderGroup(showNewData, showNewData ? $"[{language}]: Скрыть новые данные" : $"[{language}]: Отобразить новые данные");
        if (!showNewData)
        {
            EditorGUILayout.EndFoldoutHeaderGroup();
            return;
        }


        int newCount = Mathf.Max(0, EditorGUILayout.IntField("Size", newItems.Count, GUILayout.Width(100)));
        while (newCount < newItems.Count)
            newItems.RemoveAt(newItems.Count - 1);
        while (newCount > newItems.Count)
            newItems.Add(new LocalizationItem { key = "", value = ""});

        for(int i =0; i < newItems.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            newItems[i].key = EditorGUILayout.TextField("Key:", newItems[i].key);
            newItems[i].value = EditorGUILayout.TextField("Value:", newItems[i].value);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("Добавить в данные"))
        {
            AddNewItems();
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    /// <summary>
    /// Отображение настроек для добавления в другой словарь
    /// </summary>
    private void DrawMergeData()
    {
        showMerge = EditorGUILayout.BeginFoldoutHeaderGroup(showMerge, showMerge ? $"[{mergeLanguage}]: Скрыть настройки добавления в другой словарь" : $"[{mergeLanguage}]: Отобразить настройки добавления в другой словарь");
        if (showMerge)
        {
            EditorGUIUtility.labelWidth = defaultLabelWidth;
            mergeLanguage = (SystemLanguage)EditorGUILayout.EnumPopup("Добавить в:", mergeLanguage);

            if (GUILayout.Button("Добавить новые в словарь"))
            {
                if (mergeLanguage == language)
                {
                    Debug.Log("Невозможно объединять словари одного и того же языка");
                }
                else
                {
                    AddToDictionary(in items, mergeLanguage);
                    if (EditorUtility.DisplayDialog("После добавления:", $"[{mergeLanguage}]: Переключиться на язык?", "Да", "Нет"))
                    {
                        language = mergeLanguage;
                    }
                }
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    /// <summary>
    /// Загрузка из файла локализации
    /// </summary>
    private static void LoadLocalization(SystemLanguage lang, ref List<LocalizationItem> itemsList, bool updateEnableEditList)
    {
        if (LocalizationManager.AvailableLanguages.All(el => el != lang))
            throw new RapWayException($"Язык [{lang}] не поддерживается");

        Debug.Log($"Load localization for [{language}] language");
        string jsonData = File.ReadAllText(LocalizationManager.WindowsFileName(lang));

        ParseLocalizationData(jsonData, ref itemsList, updateEnableEditList);
    }

    private static void ParseLocalizationData(string jsonData, ref List<LocalizationItem> itemsList, bool updateEnableEditList)
    {
        if (string.IsNullOrEmpty(jsonData))
            throw new RapWayException("Не найден файл локализации!");

        var data = JsonUtility.FromJson<LocalizationData>(jsonData);
        if(data.items == null)
        {
            itemsList = new List<LocalizationItem>();
            if(updateEnableEditList)
                enableEditData = new List<bool>();
        }
        else
        {
            itemsList = new List<LocalizationItem>(data.items);
            if(updateEnableEditList)
                enableEditData.AddRange(Enumerable.Repeat(false, itemsList.Count));
        }
    }

    /// <summary>
    /// Сохранение в файл локализации
    /// </summary>
    private static void SaveLocalization(SystemLanguage lang, in List<LocalizationItem> itemsList)
    {
        if (LocalizationManager.AvailableLanguages.All(el => el != lang))
            throw new RapWayException($"Язык [{lang}] не поддерживается");

        Debug.Log($"Save localization for [{language}] language");

        string jsonData = LocalizationManager.LocalizationDataToJson(new LocalizationData { items = itemsList.ToArray()});
        File.WriteAllText(LocalizationManager.WindowsFileName(lang), jsonData);
    }

    private void AddNewItems()
    {
        if (newItems.Count == 0)
            return;

        Debug.ClearDeveloperConsole();

        for(int index = newItems.Count - 1; index >= 0; index--)
        {
            newItems[index].key = newItems[index].key.Trim();
            newItems[index].value = newItems[index].value.Trim();

            bool keyNull = string.IsNullOrWhiteSpace(newItems[index].key), valueNull = string.IsNullOrWhiteSpace(newItems[index].value);
            if(keyNull || valueNull)
            {
                if(keyNull && valueNull)
                {
                    newItems.RemoveAt(index);
                    continue;
                }

                Debug.Log($"One of item's [key: '{newItems[index].key}' value: '{newItems[index].value}'] field is empty");
                continue;
            }

            if (items.Any(it => it.key == newItems[index].key))
            {
                Debug.LogError($"Item with key: [{newItems[index].key}] is already exists!");
                continue;
            }

            items.Add(newItems[index]);
            enableEditData.Add(false);
            newItems.RemoveAt(index);
        }
        if (autoSave)
            SaveLocalization(language, in items);

        Repaint();
    }

    private void AddToDictionary(in List<LocalizationItem> sourceItems, SystemLanguage destinationLanguage)
    {
        List<LocalizationItem> destinationItems = new List<LocalizationItem>();
        LoadLocalization(destinationLanguage, ref destinationItems, false);
        destinationItems.AddRange(
            sourceItems.Where(it => destinationItems.All(dit => dit.key != it.key))//Добавить из источника такие, ключей каких нет в целевом
        );
        SaveLocalization(destinationLanguage, destinationItems);
    }

}