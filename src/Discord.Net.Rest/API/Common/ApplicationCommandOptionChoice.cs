using Newtonsoft.Json;

namespace Discord.API
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ApplicationCommandOptionChoice
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public object Value { get; set; }
    }
}
