using System;
using System.Collections.Generic;
using System.IO;
using RapWay.Presentation.Unity.Localization;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace RapWay.Composition.Unity.Editor.Localization
{
    public static class LocalizationCatalogBootstrapper
    {
        private const string RootFolder = "Assets/_Project/Localization";
        private const string LocalesFolder = "Assets/_Project/Localization/Locales";
        private const string TablesFolder = "Assets/_Project/Localization/StringTables";
        private const string SettingsAssetPath = "Assets/_Project/Localization/LocalizationSettings.asset";
        private const string MissingTranslationMessage = "[MISSING:{table.TableCollectionName}.{key}]";

        [MenuItem("Rap Way/Localization/Bootstrap Catalog")]
        public static void BootstrapCatalog()
        {
            BootstrapCatalogInternal(logToConsole: true);
        }

        [MenuItem("Rap Way/Localization/Validate Catalog")]
        public static void ValidateCatalog()
        {
            try
            {
                ValidateCatalogInternal();
                Debug.Log("Rap Way localization catalog validation passed.");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                throw;
            }
        }

        public static void BootstrapCatalogBatchMode()
        {
            try
            {
                BootstrapCatalogInternal(logToConsole: true);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorApplication.Exit(1);
                return;
            }

            EditorApplication.Exit(0);
        }

        private static void BootstrapCatalogInternal(bool logToConsole)
        {
            EnsureFolder("Assets", "_Project");
            EnsureFolder("Assets/_Project", "Localization");
            EnsureFolder(RootFolder, "Locales");
            EnsureFolder(RootFolder, "StringTables");

            LocalizationSettings settings = EnsureSettingsAsset();
            Locale russianLocale = EnsureLocaleAsset("ru", SystemLanguage.Russian, "Russian");
            Locale englishLocale = EnsureLocaleAsset("en", SystemLanguage.English, "English");

            EnsureLocaleRegistered(russianLocale);
            EnsureLocaleRegistered(englishLocale);
            ConfigureSettings(settings, russianLocale);

            StringTableCollection collection = EnsureStringTableCollection(
                GameLocalizationTables.UiShell,
                new List<Locale> { russianLocale, englishLocale });

            SynchronizeCollection(collection, russianLocale, englishLocale);
            ValidateCatalogInternal();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (logToConsole)
            {
                Debug.Log("Rap Way localization catalog bootstrap completed.");
            }
        }

        private static LocalizationSettings EnsureSettingsAsset()
        {
            LocalizationSettings settings = AssetDatabase.LoadAssetAtPath<LocalizationSettings>(SettingsAssetPath);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<LocalizationSettings>();
                settings.name = "Rap Way Localization Settings";
                AssetDatabase.CreateAsset(settings, SettingsAssetPath);
            }

            if (LocalizationEditorSettings.ActiveLocalizationSettings != settings)
            {
                LocalizationEditorSettings.ActiveLocalizationSettings = settings;
            }

            EditorUtility.SetDirty(settings);
            return settings;
        }

        private static Locale EnsureLocaleAsset(string localeCode, SystemLanguage systemLanguage, string fileName)
        {
            Locale locale = FindProjectLocale(localeCode);
            if (locale != null)
            {
                return locale;
            }

            locale = Locale.CreateLocale(systemLanguage);
            string assetPath = $"{LocalesFolder}/{fileName}.asset";
            AssetDatabase.CreateAsset(locale, assetPath);
            EditorUtility.SetDirty(locale);
            return locale;
        }

        private static Locale FindProjectLocale(string localeCode)
        {
            Locale locale = LocalizationEditorSettings.GetLocale(localeCode);
            if (locale != null)
            {
                return locale;
            }

            string[] guids = AssetDatabase.FindAssets("t:Locale");
            for (int index = 0; index < guids.Length; index++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[index]);
                Locale candidate = AssetDatabase.LoadAssetAtPath<Locale>(assetPath);
                if (candidate != null && string.Equals(candidate.Identifier.Code, localeCode, StringComparison.OrdinalIgnoreCase))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static void EnsureLocaleRegistered(Locale locale)
        {
            IReadOnlyCollection<Locale> locales = LocalizationEditorSettings.GetLocales();
            foreach (Locale existing in locales)
            {
                if (existing == locale)
                {
                    return;
                }
            }

            LocalizationEditorSettings.AddLocale(locale);
        }

        private static void ConfigureSettings(LocalizationSettings settings, Locale projectLocale)
        {
            LocalizationEditorSettings.ActiveLocalizationSettings = settings;
            LocalizationSettings.ProjectLocale = projectLocale;
            LocalizationSettings.InitializeSynchronously = true;
            settings.GetStringDatabase().NoTranslationFoundMessage = MissingTranslationMessage;

            List<IStartupLocaleSelector> selectors = settings.GetStartupLocaleSelectors();
            for (int index = 0; index < selectors.Count; index++)
            {
                if (selectors[index] is SpecificLocaleSelector specificLocaleSelector)
                {
                    specificLocaleSelector.LocaleId = projectLocale.Identifier;
                }
            }

            EditorUtility.SetDirty(settings);
        }

        private static StringTableCollection EnsureStringTableCollection(string collectionName, IList<Locale> locales)
        {
            StringTableCollection collection = LocalizationEditorSettings.GetStringTableCollection(collectionName);
            if (collection == null)
            {
                collection = LocalizationEditorSettings.CreateStringTableCollection(collectionName, TablesFolder, locales);
            }

            for (int index = 0; index < locales.Count; index++)
            {
                Locale locale = locales[index];
                StringTable table = collection.GetTable(locale.Identifier) as StringTable;
                if (table == null)
                {
                    table = collection.AddNewTable(locale.Identifier) as StringTable;
                }

                LocalizationEditorSettings.SetPreloadTableFlag(table, true);
                EditorUtility.SetDirty(table);
            }

            return collection;
        }

        private static void SynchronizeCollection(StringTableCollection collection, Locale russianLocale, Locale englishLocale)
        {
            StringTable russianTable = collection.GetTable(russianLocale.Identifier) as StringTable;
            StringTable englishTable = collection.GetTable(englishLocale.Identifier) as StringTable;

            if (russianTable == null || englishTable == null)
            {
                throw new InvalidOperationException($"Collection '{collection.TableCollectionName}' is missing required locale tables.");
            }

            foreach (GameLocalizationSeedEntry entry in GameLocalizationSeedData.Entries)
            {
                UpsertEntry(russianTable, entry.Key.EntryKey, entry.RussianText, entry.IsSmart);
                UpsertEntry(englishTable, entry.Key.EntryKey, entry.EnglishText, entry.IsSmart);
            }

            EditorUtility.SetDirty(russianTable);
            EditorUtility.SetDirty(englishTable);
            EditorUtility.SetDirty(russianTable.SharedData);
            EditorUtility.SetDirty(englishTable.SharedData);
        }

        private static void UpsertEntry(StringTable table, string entryKey, string text, bool isSmart)
        {
            StringTableEntry entry = table.GetEntry(entryKey);
            if (entry == null)
            {
                entry = table.AddEntry(entryKey, text);
            }

            entry.Value = text ?? string.Empty;
            entry.IsSmart = isSmart;
        }

        private static void ValidateCatalogInternal()
        {
            StringTableCollection collection = LocalizationEditorSettings.GetStringTableCollection(GameLocalizationTables.UiShell);
            if (collection == null)
            {
                throw new InvalidOperationException($"String Table Collection '{GameLocalizationTables.UiShell}' was not found.");
            }

            Locale russianLocale = LocalizationEditorSettings.GetLocale("ru");
            Locale englishLocale = LocalizationEditorSettings.GetLocale("en");
            if (russianLocale == null || englishLocale == null)
            {
                throw new InvalidOperationException("Required locales 'ru' and 'en' must both exist in the project.");
            }

            ValidateTable(collection, russianLocale.Identifier);
            ValidateTable(collection, englishLocale.Identifier);
        }

        private static void ValidateTable(StringTableCollection collection, LocaleIdentifier localeIdentifier)
        {
            StringTable table = collection.GetTable(localeIdentifier) as StringTable;
            if (table == null)
            {
                throw new InvalidOperationException($"Collection '{collection.TableCollectionName}' is missing table for locale '{localeIdentifier.Code}'.");
            }

            foreach (GameLocalizationSeedEntry entry in GameLocalizationSeedData.Entries)
            {
                StringTableEntry tableEntry = table.GetEntry(entry.Key.EntryKey);
                if (tableEntry == null)
                {
                    throw new InvalidOperationException($"Missing entry '{entry.Key.EntryKey}' in table '{collection.TableCollectionName}' for locale '{localeIdentifier.Code}'.");
                }
            }
        }

        private static void EnsureFolder(string parentFolder, string childFolderName)
        {
            string expectedPath = $"{parentFolder}/{childFolderName}";
            if (AssetDatabase.IsValidFolder(expectedPath))
            {
                return;
            }

            AssetDatabase.CreateFolder(parentFolder, childFolderName);
        }
    }
}
