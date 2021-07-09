using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class CreateInteractionResponseParams
    {
        [JsonProperty("type")]
        public InteractionCallbackType Type { get; set; }
        [JsonProperty("data")]
        public Optional<InteractionApplicationCommandCallbackData> Data { get; set; }

        public CreateInteractionResponseParams (InteractionCallbackType type)
        {
            Type = type;
        }

        public CreateInteractionResponseParams (InteractionCallbackType type, InteractionApplicationCommandCallbackData data) : this(type)
        {
            Data = data;
        }
    }
}
