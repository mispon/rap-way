using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace RapWay.Presentation.Unity.Localization
{
    public sealed class GameLocalizationService : IGameLocalizationService
    {
        public string Get(LocalizationKey key)
        {
            return Get(key, Array.Empty<LocalizationArgument>());
        }

        public string Get(LocalizationKey key, params LocalizationArgument[] arguments)
        {
            if (TryGetUnityLocalizedText(key, arguments, out string localizedText))
            {
                return localizedText;
            }

            if (!GameLocalizationSeedData.TryGet(key, out GameLocalizationSeedEntry entry))
            {
                return $"[MISSING:{key.TableName}.{key.EntryKey}]";
            }

            string template = SelectFallbackText(entry);
            return ApplyArguments(template, arguments);
        }

        private static bool TryGetUnityLocalizedText(LocalizationKey key, LocalizationArgument[] arguments, out string localizedText)
        {
            localizedText = null;

            if (!LocalizationSettings.HasSettings)
            {
                return false;
            }

            try
            {
                LocalizationSettings.InitializeSynchronously = true;

                if (arguments == null || arguments.Length == 0)
                {
                    localizedText = LocalizationSettings.StringDatabase.GetLocalizedString(key.TableName, key.EntryKey);
                }
                else
                {
                    Dictionary<string, object> namedArguments = BuildNamedArguments(arguments);
                    localizedText = LocalizationSettings.StringDatabase.GetLocalizedString(key.TableName, key.EntryKey, new object[] { namedArguments });
                }

                return !string.IsNullOrEmpty(localizedText);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Localization fallback activated for {key.TableName}.{key.EntryKey}: {exception.Message}");
                localizedText = null;
                return false;
            }
        }

        private static Dictionary<string, object> BuildNamedArguments(LocalizationArgument[] arguments)
        {
            Dictionary<string, object> dictionary = new(StringComparer.Ordinal);
            for (int index = 0; index < arguments.Length; index++)
            {
                LocalizationArgument argument = arguments[index];
                if (string.IsNullOrWhiteSpace(argument.Name))
                {
                    continue;
                }

                dictionary[argument.Name] = argument.Value;
            }

            return dictionary;
        }

        private static string SelectFallbackText(GameLocalizationSeedEntry entry)
        {
            return SelectFallbackLanguageCode().StartsWith("ru", StringComparison.OrdinalIgnoreCase)
                ? entry.RussianText
                : entry.EnglishText;
        }

        private static string SelectFallbackLanguageCode()
        {
            if (LocalizationSettings.HasSettings && LocalizationSettings.SelectedLocale != null)
            {
                return LocalizationSettings.SelectedLocale.Identifier.Code ?? string.Empty;
            }

            return UnityEngine.Application.systemLanguage == SystemLanguage.Russian ? "ru" : "en";
        }

        private static string ApplyArguments(string template, LocalizationArgument[] arguments)
        {
            if (arguments == null || arguments.Length == 0)
            {
                return template;
            }

            string resolved = template;
            for (int index = 0; index < arguments.Length; index++)
            {
                LocalizationArgument argument = arguments[index];
                if (string.IsNullOrWhiteSpace(argument.Name))
                {
                    continue;
                }

                resolved = resolved.Replace($"{{{argument.Name}}}", argument.GetStringValue(), StringComparison.Ordinal);
            }

            return resolved;
        }
    }
}
