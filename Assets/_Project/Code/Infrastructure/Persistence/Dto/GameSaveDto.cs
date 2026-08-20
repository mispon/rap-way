using Newtonsoft.Json;

namespace RapWay.Infrastructure.Persistence.Dto
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class GameSaveDto
    {
        [JsonProperty("schemaVersion", Order = 1, Required = Required.Always)]
        public int SchemaVersion { get; set; }

        [JsonProperty("savedAtUtc", Order = 2, Required = Required.Always)]
        public string SavedAtUtc { get; set; } = string.Empty;

        [JsonProperty("state", Order = 3, Required = Required.Always)]
        public GameStateDto State { get; set; } = new();
    }
}
