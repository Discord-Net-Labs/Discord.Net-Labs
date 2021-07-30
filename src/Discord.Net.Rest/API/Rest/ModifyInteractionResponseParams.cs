using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Rest
{
    internal class ModifyInteractionResponseParams
    {
        [JsonProperty("content")]
        public Optional<string> Content { get; set; }

        [JsonProperty("embeds")]
        public Optional<Embed[]> Embeds { get; set; }

        [JsonProperty("allowed_mentions")]
        public Optional<AllowedMentions> AllowedMentions { get; set; }

        [JsonProperty("components")]
        public Optional<API.ActionRowComponent[]> Components { get; set; }

        [JsonProperty("flags")]
        public Optional<MessageFlags?> Flags { get; set; }
    }
}
