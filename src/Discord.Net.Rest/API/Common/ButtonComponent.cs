using Newtonsoft.Json;

namespace Discord.API
{
    internal class ButtonComponent : IMessageComponent
    {
        [JsonProperty("type")]
        public ComponentType Type { get; set; }

        [JsonProperty("style")]
        public ButtonStyle Style { get; set; }

        [JsonProperty("label")]
        public Optional<string> Label { get; set; }

        [JsonProperty("emoji")]
        public Optional<Emoji> Emote { get; set; }

        [JsonProperty("custom_id")]
        public Optional<string> CustomId { get; set; }

        [JsonProperty("url")]
        public Optional<string> Url { get; set; }

        [JsonProperty("disabled")]
        public Optional<bool> Disabled { get; set; }
    }
}
