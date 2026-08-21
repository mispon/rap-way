using Newtonsoft.Json;

namespace RapWay.Infrastructure.Persistence.Dto
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class ActivitySessionDto
    {
        [JsonProperty("instanceId", Order = 1, Required = Required.Always)]
        public string InstanceId { get; set; } = string.Empty;

        [JsonProperty("definitionId", Order = 2, Required = Required.Always)]
        public string DefinitionId { get; set; } = string.Empty;

        [JsonProperty("startedAtTotalHours", Order = 3, Required = Required.Always)]
        public long StartedAtTotalHours { get; set; }

        [JsonProperty("durationHours", Order = 4, Required = Required.Always)]
        public int DurationHours { get; set; }

        [JsonProperty("elapsedHours", Order = 5, Required = Required.Always)]
        public int ElapsedHours { get; set; }
    }
}
