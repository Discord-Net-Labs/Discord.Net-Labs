using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ApplicationCommandInteractionDataResolved
    {
        [JsonProperty("users")]
        public Optional<IDictionary<ulong, User>> Users { get; set; }
        [JsonProperty("members")]
        public Optional<IDictionary<ulong, GuildMember>> Members { get; set; }
        [JsonProperty("roles")]
        public Optional<IDictionary<ulong, Role>> Roles { get; set; }
        [JsonProperty("channels")]
        public Optional<IDictionary<ulong, Channel>> Channels { get; set; }
    }
}
