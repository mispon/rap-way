using Newtonsoft.Json;

namespace RapWay.Infrastructure.Persistence.Dto
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class StatusEffectDto
    {
        [JsonProperty("instanceId", Order = 1, Required = Required.Always)]
        public string InstanceId { get; set; } = string.Empty;

        [JsonProperty("definitionId", Order = 2, Required = Required.Always)]
        public string DefinitionId { get; set; } = string.Empty;

        [JsonProperty("sourceId", Order = 3, Required = Required.Always)]
        public string SourceId { get; set; } = string.Empty;

        [JsonProperty("appliedAtTotalHours", Order = 4, Required = Required.Always)]
        public long AppliedAtTotalHours { get; set; }

        [JsonProperty("expiresAtTotalHours", Order = 5, Required = Required.AllowNull)]
        public long? ExpiresAtTotalHours { get; set; }

        [JsonProperty("stacks", Order = 6, Required = Required.Always)]
        public int Stacks { get; set; }
    }
}
