using Newtonsoft.Json;

namespace Discord.API
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class ApplicationCommandPermission
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("type")]
        public ApplicationCommandPermissionType Type { get; set; }
        [JsonProperty("permission")]
        public bool Allow { get; set; }
    }
}
