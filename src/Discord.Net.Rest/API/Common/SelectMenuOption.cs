using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class SelectMenuOption
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

        public SelectMenuOption() { }

        public SelectMenuOption(Discord.SelectMenuOption option)
        {
            this.Label = option.Label;
            this.Value = option.Value;
            this.Description = option.Description;

            if (option.Emoji != null)
            {
                if (option.Emoji is CustomEmoji e)
                {
                    this.Emoji = new Emoji()
                    {
                        Name = e.Name,
                        Animated = e.Animated,
                        Id = e.Id,
                    };
                }
                else
                {
                    this.Emoji = new Emoji()
                    {
                        Name = option.Emoji.Name
                    };
                }
            }

            this.Default = option.Default.HasValue ? option.Default.Value : Optional<bool>.Unspecified;
        }
    }
}
