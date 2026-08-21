using System.Collections.Generic;
using Newtonsoft.Json;

namespace RapWay.Infrastructure.Persistence.Dto
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class CharacterStateDto
    {
        [JsonProperty("id", Order = 1, Required = Required.Always)]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("startTemplateId", Order = 2, Required = Required.Always)]
        public string StartTemplateId { get; set; } = string.Empty;

        [JsonProperty("energy", Order = 3, Required = Required.Always)]
        public BoundedResourceDto Energy { get; set; } = new();

        [JsonProperty("satiety", Order = 4, Required = Required.Always)]
        public BoundedResourceDto Satiety { get; set; } = new();

        [JsonProperty("motivation", Order = 5, Required = Required.Always)]
        public BoundedResourceDto Motivation { get; set; } = new();

        [JsonProperty("walletMinorUnits", Order = 6, Required = Required.Always)]
        public string WalletMinorUnits { get; set; } = string.Empty;

        [JsonProperty("audience", Order = 7, Required = Required.Always)]
        public List<AudienceSegmentDto> Audience { get; set; } = new();

        [JsonProperty("hype", Order = 8, Required = Required.Always)]
        public BoundedResourceDto Hype { get; set; } = new();

        [JsonProperty("skills", Order = 9, Required = Required.Always)]
        public List<SkillProgressDto> Skills { get; set; } = new();

        [JsonProperty("talentIds", Order = 10, Required = Required.Always)]
        public List<string> TalentIds { get; set; } = new();

        [JsonProperty("statusEffects", Order = 11, Required = Required.Always)]
        public List<StatusEffectDto> StatusEffects { get; set; } = new();
    }
}
