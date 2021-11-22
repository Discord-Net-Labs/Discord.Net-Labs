using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public struct GuildMember : ICachedUser, ICacheModel
    {
        public ulong GuildId { get; set; }
        public string Nickname { get; set; }
        public string GuildAvatar { get; set; }
        public ulong[] RoleIds { get; set; }
        public long? JoinedAt { get; set; }
        public VoiceState? VoiceState { get; set; }
        public bool? Pending { get; set; }
        public long? PremiumSince { get; set; }

        public ulong Id { get; set; }
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public bool IsBot { get; set; }
        public string Avatar { get; set; }

        public CurrentUser? CurrentUser { get; set; }
    }
}
