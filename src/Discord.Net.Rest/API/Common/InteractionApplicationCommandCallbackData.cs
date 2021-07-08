using Newtonsoft.Json;

namespace Discord.API
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class InteractionApplicationCommandCallbackData
    {
        [JsonProperty("tts")]
        public Optional<bool> TTS { get; set; }
        [JsonProperty("content")]
        public Optional<string> Content { get; set; }
        [JsonProperty("embeds")]
        public Optional<Embed[]> Embeds { get; set; }
        [JsonProperty("allowed_mentions")]
        public Optional<AllowedMentions> AllowedMentions { get; set; }
        [JsonProperty("flags")]
        public Optional<InteractionApplicationCommandCallbackFlags> Flags { get; set; }
        [JsonProperty("components")]
        public Optional<MessageComponent[]> Components { get; set; }
    }
}
