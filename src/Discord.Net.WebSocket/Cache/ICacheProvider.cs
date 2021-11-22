using Discord.WebSocket.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public interface ICacheProvider
    {
        #region Users

        ICachedUser GetUser(ulong id);
        GuildMember? GetGuildUser(ulong id, ulong guildId);
        ThreadUser? GetThreadUser(ulong id, ulong threadId, ulong guildId);
        IEnumerable<ThreadUser> GetThreadUsers(ulong threadId, ulong guildId);
        IEnumerable<GuildMember> GetGuildUsers(ulong guildId);

        void UpdateThreadUser(ThreadUser model);
        void UpdateGuildUser(GuildMember model);
        void UpdateUser(ICachedUser model);

        void PurgeGuildUsers(ulong guildId);
        void PurgeThreadUsers(ulong threadId, ulong guildId);

        GuildMember? DeleteGuildMember(ulong userId, ulong guildId);
        ICachedUser DeleteUser(ulong userId);
        ThreadUser? DeleteThreadMember(ulong userId, ulong threadId, ulong guildId);

        #endregion
    }
}
