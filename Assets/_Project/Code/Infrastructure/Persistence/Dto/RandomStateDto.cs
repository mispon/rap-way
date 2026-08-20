using System.Collections.Generic;
using Newtonsoft.Json;

namespace RapWay.Infrastructure.Persistence.Dto
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class RandomStateDto
    {
        [JsonProperty("masterSeed", Order = 1, Required = Required.Always)]
        public string MasterSeed { get; set; } = string.Empty;

        [JsonProperty("streams", Order = 2, Required = Required.Always)]
        public List<RandomStreamStateDto> Streams { get; set; } = new();
    }
}
