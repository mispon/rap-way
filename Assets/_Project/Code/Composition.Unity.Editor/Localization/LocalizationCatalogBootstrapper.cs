using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        private const string GeneratedKeyApiPath = "Assets/_Project/Code/Presentation.Unity/Localization/GameLocalizationKeys.cs";
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

        [MenuItem("Rap Way/Localization/Generate Key API")]
        public static void GenerateKeyApi()
        {
            BootstrapCatalogInternal(logToConsole: false);
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

            ValidateCatalogInternal();
            GenerateKeyApi(collection);

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

            foreach (SharedTableData.SharedTableEntry sharedEntry in collection.SharedData.Entries)
            {
                StringTableEntry entry = table.GetEntry(sharedEntry.Key);
                if (entry == null || string.IsNullOrWhiteSpace(entry.Value))
                {
                    throw new InvalidOperationException($"Entry '{sharedEntry.Key}' is missing or empty in table '{collection.TableCollectionName}' for locale '{localeIdentifier.Code}'.");
                }
            }
        }

        private static void GenerateKeyApi(StringTableCollection collection)
        {
            StringBuilder source = new();
            source.AppendLine("// <auto-generated />");
            source.AppendLine("using RapWay.Domain.Localization;");
            source.AppendLine();
            source.AppendLine("namespace RapWay.Presentation.Unity.Localization");
            source.AppendLine("{");
            source.AppendLine("    public static class GameLocalizationTables");
            source.AppendLine("    {");
            source.Append("        public const string ");
            source.Append(ToPascalIdentifier(collection.TableCollectionName));
            source.Append(" = \"");
            source.Append(collection.TableCollectionName);
            source.AppendLine("\";");
            source.AppendLine("    }");
            source.AppendLine();
            source.AppendLine("    public static class GameLocalizationKeys");
            source.AppendLine("    {");
            source.Append("        public static class ");
            source.AppendLine(ToPascalIdentifier(collection.TableCollectionName));
            source.AppendLine("        {");

            List<SharedTableData.SharedTableEntry> entries = new(collection.SharedData.Entries);
            entries.Sort((left, right) => string.CompareOrdinal(left.Key, right.Key));
            HashSet<string> memberNames = new(StringComparer.Ordinal);
            for (int index = 0; index < entries.Count; index++)
            {
                SharedTableData.SharedTableEntry entry = entries[index];
                string memberName = ToPascalIdentifier(entry.Key);
                if (!memberNames.Add(memberName))
                {
                    throw new InvalidOperationException($"Localization key '{entry.Key}' maps to a duplicate generated member '{memberName}'.");
                }

                source.Append("            public static readonly LocalizationKey ");
                source.Append(memberName);
                source.Append(" = new(GameLocalizationTables.");
                source.Append(ToPascalIdentifier(collection.TableCollectionName));
                source.Append(", \"");
                source.Append(entry.Key);
                source.AppendLine("\");");
            }

            source.AppendLine("        }");
            source.AppendLine("    }");
            source.AppendLine("}");

            string generatedSource = source.ToString();
            if (File.Exists(GeneratedKeyApiPath) && string.Equals(File.ReadAllText(GeneratedKeyApiPath), generatedSource, StringComparison.Ordinal))
            {
                return;
            }

            File.WriteAllText(GeneratedKeyApiPath, generatedSource, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            AssetDatabase.ImportAsset(GeneratedKeyApiPath);
        }

        private static string ToPascalIdentifier(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("A localization key is required.", nameof(value));
            }

            StringBuilder result = new();
            bool capitalizeNext = true;
            for (int index = 0; index < value.Length; index++)
            {
                char character = value[index];
                if (character == '_' || character == '-' || character == '.')
                {
                    capitalizeNext = true;
                    continue;
                }

                if (!char.IsLetterOrDigit(character))
                {
                    throw new ArgumentException($"Localization key '{value}' cannot be converted to a C# identifier.", nameof(value));
                }

                if (result.Length == 0 && char.IsDigit(character))
                {
                    result.Append('_');
                }

                result.Append(capitalizeNext ? char.ToUpperInvariant(character) : char.ToLowerInvariant(character));
                capitalizeNext = false;
            }

            if (result.Length == 0)
            {
                throw new ArgumentException($"Localization key '{value}' cannot be converted to a C# identifier.", nameof(value));
            }

            return result.ToString();
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
