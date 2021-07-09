using Newtonsoft.Json;

namespace Discord.API
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class MessageComponent
    {
        [JsonProperty("type")]
        public MessageComponentType Type { get; set; }
        [JsonProperty("style")]
        public Optional<ButtonStyles> Style { get; set; }
        [JsonProperty("label")]
        public Optional<string> Label { get; set; }
        [JsonProperty("emoji")]
        public Optional<Emoji> Emoji { get; set; }
        [JsonProperty("custom_id")]
        public Optional<string> CustomId { get; set; }
        [JsonProperty("url")]
        public Optional<string> Url { get; set; }
        [JsonProperty("disabled")]
        public Optional<bool> Disabled { get; set; }
        [JsonProperty("placeholder")]
        public Optional<string> Placeholder { get; set; }
        [JsonProperty("min_values")]
        public Optional<int> MinValues { get; set; }
        [JsonProperty("max_values")]
        public Optional<int> MaxValues { get; set; }
        [JsonProperty("options")]
        public Optional<SelectOption[]> Options { get; set; }
        [JsonProperty("components")]
        public Optional<MessageComponent[]> Components { get; set; }
    }
}
