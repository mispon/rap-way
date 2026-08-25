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
        private const string CollectionArgumentName = "-rapwayLocalizationCollection";
        private const string KeyArgumentName = "-rapwayLocalizationKey";
        private const string RussianValueArgumentName = "-rapwayLocalizationRussian";
        private const string EnglishValueArgumentName = "-rapwayLocalizationEnglish";
        private const string EntriesBase64ArgumentName = "-rapwayLocalizationEntriesBase64";

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

        public static void UpsertEntryBatchMode()
        {
            try
            {
                BootstrapCatalogInternal(logToConsole: false);

                string collectionName = GetRequiredCommandLineArgument(CollectionArgumentName);
                Locale russianLocale = LocalizationEditorSettings.GetLocale("ru") ??
                                      throw new InvalidOperationException("Required locale 'ru' was not found.");
                Locale englishLocale = LocalizationEditorSettings.GetLocale("en") ??
                                      throw new InvalidOperationException("Required locale 'en' was not found.");
                StringTableCollection collection = EnsureStringTableCollection(
                    collectionName,
                    new List<Locale> { russianLocale, englishLocale });

                IReadOnlyList<LocalizationEntryInput> entries = GetEntryInputs();
                for (int index = 0; index < entries.Count; index++)
                {
                    LocalizationEntryInput entry = entries[index];
                    UpsertEntry(collection, russianLocale.Identifier, entry.Key, entry.RussianValue);
                    UpsertEntry(collection, englishLocale.Identifier, entry.Key, entry.EnglishValue);
                }

                ValidateCatalogInternal();
                GenerateKeyApi(FindProjectStringTableCollections());

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log($"Rap Way localization entries for '{collectionName}' were updated.");
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
            GenerateKeyApi(FindProjectStringTableCollections());

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
            SpecificLocaleSelector specificLocaleSelector = null;
            for (int index = 0; index < selectors.Count; index++)
            {
                if (selectors[index] is SpecificLocaleSelector selector)
                {
                    specificLocaleSelector = selector;
                    specificLocaleSelector.LocaleId = projectLocale.Identifier;
                    break;
                }
            }

            if (specificLocaleSelector != null)
            {
                selectors.Remove(specificLocaleSelector);

                int commandLineSelectorIndex = selectors.FindIndex(
                    selector => selector is CommandLineLocaleSelector);
                selectors.Insert(commandLineSelectorIndex + 1, specificLocaleSelector);
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
            Locale russianLocale = LocalizationEditorSettings.GetLocale("ru");
            Locale englishLocale = LocalizationEditorSettings.GetLocale("en");
            if (russianLocale == null || englishLocale == null)
            {
                throw new InvalidOperationException("Required locales 'ru' and 'en' must both exist in the project.");
            }

            IReadOnlyList<StringTableCollection> collections = FindProjectStringTableCollections();
            if (collections.Count == 0)
            {
                throw new InvalidOperationException("No String Table Collections were found.");
            }

            for (int index = 0; index < collections.Count; index++)
            {
                StringTableCollection collection = collections[index];
                ValidateTable(collection, russianLocale.Identifier);
                ValidateTable(collection, englishLocale.Identifier);
            }
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

        private static void GenerateKeyApi(IReadOnlyList<StringTableCollection> collections)
        {
            StringBuilder source = new();
            source.AppendLine("// <auto-generated />");
            source.AppendLine("using RapWay.Domain.Localization;");
            source.AppendLine();
            source.AppendLine("namespace RapWay.Presentation.Unity.Localization");
            source.AppendLine("{");
            source.AppendLine("    public static class GameLocalizationTables");
            source.AppendLine("    {");
            for (int index = 0; index < collections.Count; index++)
            {
                StringTableCollection collection = collections[index];
                source.Append("        public const string ");
                source.Append(ToPascalIdentifier(collection.TableCollectionName));
                source.Append(" = \"");
                source.Append(collection.TableCollectionName);
                source.AppendLine("\";");
            }

            source.AppendLine("    }");
            source.AppendLine();
            source.AppendLine("    public static class GameLocalizationKeys");
            source.AppendLine("    {");
            for (int collectionIndex = 0; collectionIndex < collections.Count; collectionIndex++)
            {
                StringTableCollection collection = collections[collectionIndex];
                source.Append("        public static class ");
                source.AppendLine(ToPascalIdentifier(collection.TableCollectionName));
                source.AppendLine("        {");

                List<SharedTableData.SharedTableEntry> entries = new(collection.SharedData.Entries);
                entries.Sort((left, right) => string.CompareOrdinal(left.Key, right.Key));
                HashSet<string> memberNames = new(StringComparer.Ordinal);
                for (int entryIndex = 0; entryIndex < entries.Count; entryIndex++)
                {
                    SharedTableData.SharedTableEntry entry = entries[entryIndex];
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
            }
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

        private static IReadOnlyList<StringTableCollection> FindProjectStringTableCollections()
        {
            string[] guids = AssetDatabase.FindAssets("t:StringTableCollection", new[] { TablesFolder });
            List<StringTableCollection> collections = new(guids.Length);
            for (int index = 0; index < guids.Length; index++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[index]);
                StringTableCollection collection = AssetDatabase.LoadAssetAtPath<StringTableCollection>(assetPath);
                if (collection != null)
                {
                    collections.Add(collection);
                }
            }

            collections.Sort((left, right) => string.CompareOrdinal(left.TableCollectionName, right.TableCollectionName));
            return collections;
        }

        private static void UpsertEntry(
            StringTableCollection collection,
            LocaleIdentifier localeIdentifier,
            string key,
            string value)
        {
            StringTable table = collection.GetTable(localeIdentifier) as StringTable ??
                                throw new InvalidOperationException(
                                    $"Collection '{collection.TableCollectionName}' is missing table '{localeIdentifier.Code}'.");
            StringTableEntry entry = table.GetEntry(key) ?? table.AddEntry(key, value);
            entry.Value = value;
            entry.IsSmart = value.IndexOf('{') >= 0 && value.IndexOf('}') >= 0;
            EditorUtility.SetDirty(table);
            EditorUtility.SetDirty(collection.SharedData);
        }

        private static string GetRequiredCommandLineArgument(string argumentName)
        {
            string[] arguments = Environment.GetCommandLineArgs();
            for (int index = 0; index < arguments.Length - 1; index++)
            {
                if (string.Equals(arguments[index], argumentName, StringComparison.Ordinal))
                {
                    string value = arguments[index + 1];
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        return value;
                    }
                }
            }

            throw new ArgumentException($"Command-line argument '{argumentName}' is required.", nameof(argumentName));
        }

        private static IReadOnlyList<LocalizationEntryInput> GetEntryInputs()
        {
            if (!TryGetCommandLineArgument(EntriesBase64ArgumentName, out string encodedEntries))
            {
                return new[]
                {
                    new LocalizationEntryInput(
                        GetRequiredCommandLineArgument(KeyArgumentName),
                        GetRequiredCommandLineArgument(RussianValueArgumentName),
                        GetRequiredCommandLineArgument(EnglishValueArgumentName))
                };
            }

            string decodedEntries;
            try
            {
                decodedEntries = Encoding.UTF8.GetString(Convert.FromBase64String(encodedEntries));
            }
            catch (FormatException exception)
            {
                throw new ArgumentException("Localization entries must be valid Base64 UTF-8 text.", EntriesBase64ArgumentName, exception);
            }

            string[] lines = decodedEntries.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            List<LocalizationEntryInput> entries = new(lines.Length);
            for (int index = 0; index < lines.Length; index++)
            {
                string[] fields = lines[index].Split(new[] { '\t' });
                if (fields.Length != 3 || string.IsNullOrWhiteSpace(fields[0]) || string.IsNullOrWhiteSpace(fields[1]) || string.IsNullOrWhiteSpace(fields[2]))
                {
                    throw new ArgumentException("Each localization entry must contain a key, Russian value, and English value separated by tabs.", EntriesBase64ArgumentName);
                }

                entries.Add(new LocalizationEntryInput(fields[0], fields[1], fields[2]));
            }

            if (entries.Count == 0)
            {
                throw new ArgumentException("At least one localization entry is required.", EntriesBase64ArgumentName);
            }

            return entries;
        }

        private static bool TryGetCommandLineArgument(string argumentName, out string value)
        {
            string[] arguments = Environment.GetCommandLineArgs();
            for (int index = 0; index < arguments.Length - 1; index++)
            {
                if (string.Equals(arguments[index], argumentName, StringComparison.Ordinal) &&
                    !string.IsNullOrWhiteSpace(arguments[index + 1]))
                {
                    value = arguments[index + 1];
                    return true;
                }
            }

            value = string.Empty;
            return false;
        }

        private readonly struct LocalizationEntryInput
        {
            public LocalizationEntryInput(string key, string russianValue, string englishValue)
            {
                Key = key;
                RussianValue = russianValue;
                EnglishValue = englishValue;
            }

            public string Key { get; }

            public string RussianValue { get; }

            public string EnglishValue { get; }
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
