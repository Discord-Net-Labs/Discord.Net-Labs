using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class GuildScheduledEventEntityMetadata
    {
        [JsonProperty("speaker_ids")]
        public Optional<ulong[]> SpeakerIds { get; set; }
        [JsonProperty("location")]
        public Optional<string> Location { get; set; }
    }
}
