using Newtonsoft.Json;

namespace Discord.API
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class SelectOption
    {
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("description")]
        public Optional<string> Description { get; set; }
        [JsonProperty("emoji")]
        public Optional<Emoji> Emoji { get; set; }
        [JsonProperty("default")]
        public Optional<bool> Default { get; set; }
    }
}
