using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class DiscordError
    {
        //
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("code")]
        public JsonErrorCode Code { get; set; }
        [JsonProperty("errors")]
        public Optional<ErrorDetails[]> Errors { get; set; }
    }
}
