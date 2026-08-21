using Newtonsoft.Json;

namespace RapWay.Infrastructure.Persistence.Dto
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class AudienceSegmentDto
    {
        [JsonProperty("groupId", Order = 1, Required = Required.Always)]
        public string GroupId { get; set; } = string.Empty;

        [JsonProperty("fanCount", Order = 2, Required = Required.Always)]
        public string FanCount { get; set; } = string.Empty;
    }
}
