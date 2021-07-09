using System.IO;

namespace Discord.API
{
    internal class InteractionFollowupMessage
    {
        public string Content { get; set; }
        public Optional<string> Username { get; set; }
        public Optional<string> AvatarUrl { get; set; }
        public Optional<bool> TTS { get; set; }
        public Optional<Stream> File { get; set; }
        public Embed[] Embeds { get; set; }

    }
}
