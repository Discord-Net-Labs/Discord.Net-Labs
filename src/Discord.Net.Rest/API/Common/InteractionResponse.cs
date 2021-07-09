using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class InteractionResponse
    {
        [JsonProperty("type")]
        public InteractionCallbackType Type { get; set; }
        [JsonProperty("data")]
        public Optional<InteractionApplicationCommandCallbackData> Data { get; set; }
    }
}
