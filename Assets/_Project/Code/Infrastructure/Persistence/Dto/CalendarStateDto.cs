using Newtonsoft.Json;

namespace RapWay.Infrastructure.Persistence.Dto
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class CalendarStateDto
    {
        [JsonProperty("startYear", Order = 1, Required = Required.Always)]
        public int StartYear { get; set; }

        [JsonProperty("startMonth", Order = 2, Required = Required.Always)]
        public int StartMonth { get; set; }

        [JsonProperty("startDay", Order = 3, Required = Required.Always)]
        public int StartDay { get; set; }

        [JsonProperty("startHour", Order = 4, Required = Required.Always)]
        public int StartHour { get; set; }

        [JsonProperty("totalHours", Order = 5, Required = Required.Always)]
        public long TotalHours { get; set; }
    }
}
