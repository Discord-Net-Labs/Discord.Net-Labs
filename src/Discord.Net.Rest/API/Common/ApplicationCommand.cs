using Newtonsoft.Json;

namespace Discord.API
{
    [JsonObject(MemberSerialization= MemberSerialization.OptIn)]
    internal class ApplicationCommand
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("application_id")]
        public ulong ApplicationId { get; set; }
        [JsonProperty("guild_id")]
        public Optional<ulong> GuildId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("options")]
        public Optional<ApplicationCommandOption[]> Options { get; set; }
        [JsonProperty("default_permission")]
        public Optional<bool> DefaultPermission { get; set; }
    }
}
