using Discord.WebSocket.Cache;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public class DefaultCacheProvider : ICacheProvider
    {
        private ConcurrentDictionary<ulong, ICachedUser> _users;
        private ConcurrentDictionary<(ulong UserId, ulong GuildId), GuildMember> _guildUsers;
        private ConcurrentDictionary<(ulong UserId, ulong ThreadId, ulong GuildId), ThreadUser> _threadUsers;

        public DefaultCacheProvider()
        {
            _users = new ConcurrentDictionary<ulong, ICachedUser>(ConcurrentHashSet.DefaultConcurrencyLevel, 10);
            _guildUsers = new ConcurrentDictionary<(ulong UserId, ulong GuildId), GuildMember>(ConcurrentHashSet.DefaultConcurrencyLevel, 10);

        }

        public GuildMember? DeleteGuildMember(ulong userId, ulong guildId)
        {
            if (_guildUsers.TryRemove((userId, guildId), out var model))
                return model;
            return null;
        }
        public ThreadUser? DeleteThreadMember(ulong userId, ulong threadId, ulong guildId)
        {
            if (_threadUsers.TryRemove((userId, threadId, guildId), out var model))
                return model;
            return null;
        }
        public ICachedUser DeleteUser(ulong userId)
        {
            if (_users.TryRemove(userId, out var model))
                return model;
            return null;
        }
        public GuildMember? GetGuildUser(ulong id, ulong guildId)
        {
            if (_guildUsers.TryGetValue((id, guildId), out var model))
                return model;
            return null;
        }
        public ThreadUser? GetThreadUser(ulong id, ulong threadId, ulong guildId)
        {
            if (_threadUsers.TryGetValue((id, threadId, guildId), out var model))
                return model;
            return null;
        }
        public ICachedUser GetUser(ulong id)
        {
            if (_users.TryGetValue(id, out var model))
                return model;
            return null;
        }
        public void PurgeGuildUsers(ulong guildId)
        {
            var newCollection = _guildUsers.Where(x => x.Key.GuildId != guildId).ToArray();
            _guildUsers = new ConcurrentDictionary<(ulong UserId, ulong GuildId), GuildMember>(newCollection);
        }
        public void PurgeThreadUsers(ulong threadId, ulong guildId)
        {
            var newCollection = _threadUsers.Where(x => x.Key.ThreadId != threadId && x.Key.GuildId != guildId).ToArray();
            _threadUsers = new ConcurrentDictionary<(ulong UserId, ulong ThreadId, ulong GuildId), ThreadUser>(newCollection);
        }
        public void UpdateGuildUser(GuildMember model)
        {
            _guildUsers[(model.Id, model.GuildId)] = model;
        }
        public void UpdateThreadUser(ThreadUser model)
        {
            _threadUsers[(model.Id, model.ThreadId, model.GuildId)] = model;
        }
        public void UpdateUser(ICachedUser model)
        {
            _users[model.Id] = model;
        }
    }
}
