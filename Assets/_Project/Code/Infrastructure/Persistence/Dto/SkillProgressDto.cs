using Newtonsoft.Json;

namespace RapWay.Infrastructure.Persistence.Dto
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class SkillProgressDto
    {
        [JsonProperty("skillId", Order = 1, Required = Required.Always)]
        public string SkillId { get; set; } = string.Empty;

        [JsonProperty("experience", Order = 2, Required = Required.Always)]
        public string Experience { get; set; } = string.Empty;

        [JsonProperty("lastPracticedTotalHours", Order = 3, Required = Required.AllowNull)]
        public long? LastPracticedTotalHours { get; set; }
    }
}
