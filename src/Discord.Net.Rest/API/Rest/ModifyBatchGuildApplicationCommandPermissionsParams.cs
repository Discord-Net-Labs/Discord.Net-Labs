using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyBatchGuildApplicationCommandPermissionsParams
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("permissions")]
        public ApplicationCommandPermission[] Permissions { get; set; }

        public ModifyBatchGuildApplicationCommandPermissionsParams (ulong id, IEnumerable<ApplicationCommandPermission> permissions)
        {
            Id = id;
            Permissions = permissions.ToArray();
        }
    }
}
