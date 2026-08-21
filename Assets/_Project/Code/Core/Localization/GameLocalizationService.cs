using System;
using System.Collections.Generic;
using RapWay.Domain.Localization;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace RapWay.Core.Localization
{
    public sealed class GameLocalizationService : IGameLocalizationService
    {
        public string Get(LocalizationKey key)
        {
            return Get(key, Array.Empty<LocalizationArgument>());
        }

        public string Get(LocalizationKey key, params LocalizationArgument[] arguments)
        {
            if (!LocalizationSettings.HasSettings)
            {
                return Missing(key);
            }

            try
            {
                if (arguments == null || arguments.Length == 0)
                {
                    return LocalizationSettings.StringDatabase.GetLocalizedString(key.TableName, key.EntryKey);
                }

                return LocalizationSettings.StringDatabase.GetLocalizedString(
                    key.TableName,
                    key.EntryKey,
                    new object[] { BuildNamedArguments(arguments) });
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Localization lookup failed for {key.TableName}.{key.EntryKey}: {exception.Message}");
                return Missing(key);
            }
        }

        private static Dictionary<string, object> BuildNamedArguments(IReadOnlyList<LocalizationArgument> arguments)
        {
            Dictionary<string, object> namedArguments = new(StringComparer.Ordinal);
            for (int index = 0; index < arguments.Count; index++)
            {
                LocalizationArgument argument = arguments[index];
                namedArguments[argument.Name] = argument.Value;
            }

            return namedArguments;
        }

        private static string Missing(LocalizationKey key)
        {
            return $"[MISSING:{key.TableName}.{key.EntryKey}]";
        }
    }
}
