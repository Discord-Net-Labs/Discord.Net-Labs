using Newtonsoft.Json;

namespace Discord.API
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class GuildApplicationCommandPermission
    {
        [JsonProperty("id")]
        public ulong CommandId { get; set; }
        [JsonProperty("application_id")]
        public ulong ApplicationId { get; set; }
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
        [JsonProperty("permissions")]
        public ApplicationCommandPermission[] Permissions { get; set; }
    }
}
