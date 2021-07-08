using Newtonsoft.Json;

namespace Discord.API
{
    [JsonObject(IsReference = true, MemberSerialization = MemberSerialization.OptIn)]
    internal class ApplicationCommandOption
    {
        [JsonProperty("type")]
        public ApplicationCommandOptionType Type { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("required")]
        public Optional<bool> Required { get; set; }
        [JsonProperty("choices")]
        public Optional<ApplicationCommandOptionChoice[]> Choices { get; set; }
        [JsonProperty("options")]
        public Optional<ApplicationCommandOption[]> Options { get; set; }
    }
}
