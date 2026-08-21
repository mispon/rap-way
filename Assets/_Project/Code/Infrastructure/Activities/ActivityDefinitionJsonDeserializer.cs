#nullable enable

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RapWay.Domain.Activities;
using RapWay.Domain.Common;
using RapWay.Domain.Localization;
using RapWay.Domain.Numerics;

namespace RapWay.Infrastructure.Activities
{
    public sealed class ActivityDefinitionJsonDeserializer
    {
        private readonly JsonSerializerSettings _settings = new()
        {
            MissingMemberHandling = MissingMemberHandling.Error,
            TypeNameHandling = TypeNameHandling.None
        };

        public ActivityDefinitionCatalog Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("Activity definition JSON is required.", nameof(json));
            }

            ActivityDefinitionsDocumentDto document;
            try
            {
                document = JsonConvert.DeserializeObject<ActivityDefinitionsDocumentDto>(json, _settings) ??
                           throw new FormatException("Activity definition JSON has no document.");
            }
            catch (JsonException exception)
            {
                throw new FormatException("Activity definition JSON is invalid.", exception);
            }

            if (document.Activities == null)
            {
                throw new FormatException("Activity definition JSON has no activities collection.");
            }

            List<ActivityDefinition> definitions = new(document.Activities.Count);
            foreach (ActivityDefinitionDto dto in document.Activities)
            {
                if (dto == null)
                {
                    throw new FormatException("Activity definition JSON contains a missing activity.");
                }

                if (dto.TitleLocalization == null || dto.DescriptionLocalization == null)
                {
                    throw new FormatException("Activity definition JSON has missing localization references.");
                }

                definitions.Add(new ActivityDefinition(
                    StableId.Create(dto.Id),
                    ParseCategory(dto.Category),
                    dto.MinimumDurationHours,
                    dto.MaximumDurationHours,
                    new LocalizationKey(dto.TitleLocalization.Table, dto.TitleLocalization.Key),
                    new LocalizationKey(dto.DescriptionLocalization.Table, dto.DescriptionLocalization.Key),
                    CreateHourlyEffects(dto.HourlyEffects),
                    Money.FromMinorUnits(ParsePayment(dto.PaymentPerCompletedHourMinorUnits)),
                    string.IsNullOrWhiteSpace(dto.EventPoolId) ? default : StableId.Create(dto.EventPoolId),
                    dto.CanInterrupt,
                    dto.CanAccelerateWhenFamiliar));
            }

            return new ActivityDefinitionCatalog(definitions);
        }

        private static ActivityCategory ParseCategory(string category)
        {
            return category switch
            {
                "work" => ActivityCategory.Work,
                "rest" => ActivityCategory.Rest,
                "music" => ActivityCategory.Music,
                "social" => ActivityCategory.Social,
                "travel" => ActivityCategory.Travel,
                _ => throw new FormatException($"Activity category '{category}' is not supported.")
            };
        }

        private static ActivityHourlyEffects CreateHourlyEffects(ActivityHourlyEffectsDto? effects)
        {
            if (effects == null)
            {
                throw new FormatException("Activity hourly effects are missing.");
            }

            List<ActivitySkillExperienceGrant> grants = new(effects.SkillExperience?.Count ?? 0);
            foreach (ActivitySkillExperienceGrantDto grant in effects.SkillExperience ?? throw new FormatException("Activity skill experience grants are missing."))
            {
                if (grant == null)
                {
                    throw new FormatException("Activity skill experience grants contain a missing entry.");
                }

                grants.Add(new ActivitySkillExperienceGrant(StableId.Create(grant.SkillId), grant.ExperiencePerHour));
            }

            return new ActivityHourlyEffects(effects.EnergyChange, effects.SatietyChange, effects.MotivationChange, grants);
        }

        private static long ParsePayment(string payment)
        {
            if (!long.TryParse(payment, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out long parsed))
            {
                throw new FormatException("Activity payment per completed hour is invalid.");
            }

            return parsed;
        }

        [JsonObject(MemberSerialization.OptIn)]
        private sealed class ActivityDefinitionsDocumentDto
        {
            [JsonProperty("activities", Required = Required.Always)]
            public List<ActivityDefinitionDto>? Activities { get; set; }
        }

        [JsonObject(MemberSerialization.OptIn)]
        private sealed class ActivityDefinitionDto
        {
            [JsonProperty("id", Required = Required.Always)]
            public string Id { get; set; } = string.Empty;

            [JsonProperty("category", Required = Required.Always)]
            public string Category { get; set; } = string.Empty;

            [JsonProperty("minimumDurationHours", Required = Required.Always)]
            public int MinimumDurationHours { get; set; }

            [JsonProperty("maximumDurationHours", Required = Required.Always)]
            public int MaximumDurationHours { get; set; }

            [JsonProperty("titleLocalization", Required = Required.Always)]
            public LocalizationKeyDto? TitleLocalization { get; set; } = new();

            [JsonProperty("descriptionLocalization", Required = Required.Always)]
            public LocalizationKeyDto? DescriptionLocalization { get; set; } = new();

            [JsonProperty("hourlyEffects", Required = Required.Always)]
            public ActivityHourlyEffectsDto? HourlyEffects { get; set; } = new();

            [JsonProperty("paymentPerCompletedHourMinorUnits", Required = Required.Always)]
            public string PaymentPerCompletedHourMinorUnits { get; set; } = string.Empty;

            [JsonProperty("eventPoolId", Required = Required.AllowNull)]
            public string EventPoolId { get; set; } = string.Empty;

            [JsonProperty("canInterrupt", Required = Required.Always)]
            public bool CanInterrupt { get; set; }

            [JsonProperty("canAccelerateWhenFamiliar", Required = Required.Always)]
            public bool CanAccelerateWhenFamiliar { get; set; }
        }

        [JsonObject(MemberSerialization.OptIn)]
        private sealed class ActivityHourlyEffectsDto
        {
            [JsonProperty("energyChange", Required = Required.Always)]
            public int EnergyChange { get; set; }

            [JsonProperty("satietyChange", Required = Required.Always)]
            public int SatietyChange { get; set; }

            [JsonProperty("motivationChange", Required = Required.Always)]
            public int MotivationChange { get; set; }

            [JsonProperty("skillExperience", Required = Required.Always)]
            public List<ActivitySkillExperienceGrantDto>? SkillExperience { get; set; }
        }

        [JsonObject(MemberSerialization.OptIn)]
        private sealed class ActivitySkillExperienceGrantDto
        {
            [JsonProperty("skillId", Required = Required.Always)]
            public string SkillId { get; set; } = string.Empty;

            [JsonProperty("experiencePerHour", Required = Required.Always)]
            public int ExperiencePerHour { get; set; }
        }

        [JsonObject(MemberSerialization.OptIn)]
        private sealed class LocalizationKeyDto
        {
            [JsonProperty("table", Required = Required.Always)]
            public string Table { get; set; } = string.Empty;

            [JsonProperty("key", Required = Required.Always)]
            public string Key { get; set; } = string.Empty;
        }
    }
}
