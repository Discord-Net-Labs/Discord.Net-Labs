using Newtonsoft.Json;

namespace Discord.API
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ApplicationCommandInteractionData : IDiscordInteractionData
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("resolved")]
        public Optional<ApplicationCommandInteractionDataResolved> Resolved { get; set; }
        [JsonProperty("options")]
        public Optional<ApplicationCommandInteractionDataOption[]> Options { get; set; }
        [JsonProperty("custom_id")]
        public string CustomId { get; set; }
        [JsonProperty("component_type")]
        public ComponentType ComponentType { get; set; }
        [JsonProperty("values")]
        public Optional<string[]> Values { get; set; }
    }
}
