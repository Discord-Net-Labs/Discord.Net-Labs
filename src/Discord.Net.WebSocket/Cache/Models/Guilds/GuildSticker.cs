using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public struct GuildSticker
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public StickerFormatType FormatType { get; set; }
        public bool Available { get; set; }
        public ulong GuildId { get; set; }
        public User? User { get; set; }
    }
}
