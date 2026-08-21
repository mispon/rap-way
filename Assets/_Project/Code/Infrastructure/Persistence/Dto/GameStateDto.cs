#nullable enable

using Newtonsoft.Json;

namespace RapWay.Infrastructure.Persistence.Dto
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class GameStateDto
    {
        [JsonProperty("revision", Order = 1, Required = Required.Always)]
        public long Revision { get; set; }

        [JsonProperty("calendar", Order = 2, Required = Required.Always)]
        public CalendarStateDto Calendar { get; set; } = new();

        [JsonProperty("random", Order = 3, Required = Required.Always)]
        public RandomStateDto Random { get; set; } = new();

        [JsonProperty("character", Order = 4, Required = Required.Always)]
        public CharacterStateDto Character { get; set; } = new();

        [JsonProperty("activeActivity", Order = 5, Required = Required.AllowNull)]
        public ActivitySessionDto? ActiveActivity { get; set; }
    }
}
