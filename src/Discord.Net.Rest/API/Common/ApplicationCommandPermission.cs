using Newtonsoft.Json;

namespace Discord.API
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class ApplicationCommandPermission

    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("type")]
        public PermissionTarget Type { get; set; }

        [JsonProperty("permission")]
        public bool Permission { get; set; }
    }
}
