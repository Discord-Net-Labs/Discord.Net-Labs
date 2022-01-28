using Newtonsoft.Json;
using System;

namespace Discord.API
{
    internal class ThreadMetadata
    {
        [JsonProperty("archived")]
        public bool Archived { get; set; }

        [JsonProperty("auto_archive_duration")]
        public ThreadArchiveDuration AutoArchiveDuration { get; set; }

        [JsonProperty("archive_timestamp")]
        public DateTimeOffset ArchiveTimestamp { get; set; }

        [JsonProperty("locked")]
        public Optional<bool> Locked { get; set; }

        [JsonProperty("invitable")]
        public Optional<bool> Invitable { get; set; }

        [JsonProperty("create_timestamp")]
        public Optional<DateTimeOffset> CreatedAt { get; set; }
    }
}
