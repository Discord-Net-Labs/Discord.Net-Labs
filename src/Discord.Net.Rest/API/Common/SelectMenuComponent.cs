using Newtonsoft.Json;
using System.Linq;

namespace Discord.API
{
    internal class SelectMenuComponent : IMessageComponent
    {
        [JsonProperty("type")]
        public ComponentType Type { get; set; }

        [JsonProperty("custom_id")]
        public string CustomId { get; set; }

        [JsonProperty("options")]
        public SelectMenuOption[] Options { get; set; }

        [JsonProperty("placeholder")]
        public Optional<string> Placeholder { get; set; }

        [JsonProperty("min_values")]
        public int MinValues { get; set; }

        [JsonProperty("max_values")]
        public int MaxValues { get; set; }
    }
}
