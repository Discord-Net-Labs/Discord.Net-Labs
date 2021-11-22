using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public struct VoiceState : ICacheModel
    {
        public ulong ChannelId { get; set; }
        public string SessionId { get; set; }
        public bool Deaf { get; set; }
        public bool Mute { get; set; }
        public bool SelfDeaf { get; set; }
        public bool SelfMute { get; set; }
        public bool? SelfStream { get; set; }
        public bool SelfVideo { get; set; }
        public bool Suppress { get; set; }
        public long? RequestToSpeak { get; set; }
    }
}
