using Newtonsoft.Json;
using System.Linq;

namespace Discord.API
{
    internal class ActionRowComponent : IMessageComponent
    {
        [JsonProperty("type")]
        public ComponentType Type { get; set; }

        [JsonProperty("components")]
        public IMessageComponent[] Components { get; set; }
    }
}
