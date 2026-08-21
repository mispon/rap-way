using Newtonsoft.Json;

namespace RapWay.Infrastructure.Persistence.Dto
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class GameSaveDto
    {
        [JsonProperty("savedAtUtc", Order = 1, Required = Required.Always)]
        public string SavedAtUtc { get; set; } = string.Empty;

        [JsonProperty("state", Order = 2, Required = Required.Always)]
        public GameStateDto State { get; set; } = new();
    }
}
