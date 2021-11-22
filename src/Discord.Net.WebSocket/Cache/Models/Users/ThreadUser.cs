using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public struct ThreadUser : ICachedUser, ICacheModel
    {
        public ulong ThreadId { get; set; }
        public long ThreadJoinedAt { get; set; }
        public ulong GuildId { get; set; }
        public string Nickname { get; set; }
        public string GuildAvatar { get; set; }
        public ulong[] RoleIds { get; set; }
        public long? GuildMemberJoinedAt { get; set; }
        public VoiceState? VoiceState { get; set; }
        public bool? Pending { get; set; }
        public long? PremiumSince { get; set; }

        public ulong Id { get; set; }
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public bool IsBot { get; set; }
        public string Avatar { get; set; }
        public CurrentUser? CurrentUser { get; set; }

        internal GuildMember ToGuildMember()
            => new GuildMember()
            {
                Avatar = Avatar,
                CurrentUser = CurrentUser,
                Discriminator = Discriminator,
                GuildAvatar = GuildAvatar,
                GuildId = GuildId,
                Id = Id,
                IsBot = IsBot,
                JoinedAt = GuildMemberJoinedAt,
                Nickname = Nickname,
                Pending = Pending,
                PremiumSince = PremiumSince,
                RoleIds = RoleIds,
                Username = Username,
                VoiceState = VoiceState,
            };
    }
}
