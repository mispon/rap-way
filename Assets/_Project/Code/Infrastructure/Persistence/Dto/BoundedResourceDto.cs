using Newtonsoft.Json;

namespace RapWay.Infrastructure.Persistence.Dto
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class BoundedResourceDto
    {
        [JsonProperty("current", Order = 1, Required = Required.Always)]
        public int Current { get; set; }

        [JsonProperty("maximum", Order = 2, Required = Required.Always)]
        public int Maximum { get; set; }
    }
}
