using Newtonsoft.Json;

namespace Discord.API
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ApplicationCommandInteractionDataOption
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public ApplicationCommandOptionType Type { get; set; }
        [JsonProperty("value")]
        public Optional<object> Value { get; set; }
        [JsonProperty("options")]
        public Optional<ApplicationCommandInteractionDataOption[]> Options { get; set; }
    }
}
