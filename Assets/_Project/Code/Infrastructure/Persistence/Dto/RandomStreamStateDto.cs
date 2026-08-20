using Newtonsoft.Json;

namespace RapWay.Infrastructure.Persistence.Dto
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class RandomStreamStateDto
    {
        [JsonProperty("name", Order = 1, Required = Required.Always)]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("algorithmVersion", Order = 2, Required = Required.Always)]
        public int AlgorithmVersion { get; set; }

        [JsonProperty("state", Order = 3, Required = Required.Always)]
        public string State { get; set; } = string.Empty;
    }
}
