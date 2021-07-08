using Newtonsoft.Json;

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
