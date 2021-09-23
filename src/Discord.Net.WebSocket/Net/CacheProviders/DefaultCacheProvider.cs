using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Net.CacheProviders
{
    /// <summary>
    ///     Represents a default cache provider used with gateway models.
    /// </summary>
    public class DefaultCacheProvider : ICacheProvider
    {
        private readonly SynchronizedDictionary<ulong, IEnumerable<byte>> _channels;
        private readonly SynchronizedDictionary<ulong, IEnumerable<byte>> _guilds;
        private readonly SynchronizedDictionary<(ulong id, ulong guildId), IEnumerable<byte>> _guildChannels;
        private readonly SynchronizedDictionary<(ulong id, ulong guildId), IEnumerable<byte>> _guildUsers;
        private readonly SynchronizedDictionary<(ulong id, ulong guildId, ulong threadId), IEnumerable<byte>> _threadUsers;
        private readonly SynchronizedDictionary<(ulong id, ulong guildId), IEnumerable<byte>> _guildRoles;
        private readonly SynchronizedDictionary<(ulong id, ulong guildId), IEnumerable<byte>> _guildEmotes;
        private readonly SynchronizedDictionary<(ulong id, ulong guildId), IEnumerable<byte>> _guildStickers;
        private readonly SynchronizedDictionary<ulong, SynchronizedDictionary<ulong, IEnumerable<byte>>> _messages;
        private readonly SynchronizedDictionary<ulong, ConcurrentQueue<ulong>> _orderedMessages;
        private readonly SynchronizedDictionary<ulong, IEnumerable<byte>> _users;

        private readonly int _messageCacheSize;

        /// <summary>
        ///     Creates a new instance of the default cache provider.
        /// </summary>
        /// <param name="messageCacheSize">The size of the message cache per channel</param>
        public DefaultCacheProvider(int messageCacheSize)
        {
            _messageCacheSize = messageCacheSize;
            _channels = new SynchronizedDictionary<ulong, IEnumerable<byte>>();
            _guilds = new SynchronizedDictionary<ulong, IEnumerable<byte>>();
            _users = new SynchronizedDictionary<ulong, IEnumerable<byte>>();
            _guildChannels = new SynchronizedDictionary<(ulong id, ulong guildId), IEnumerable<byte>>();
            _guildUsers = new SynchronizedDictionary<(ulong id, ulong guildId), IEnumerable<byte>>();
            _guildEmotes = new SynchronizedDictionary<(ulong id, ulong guildId), IEnumerable<byte>>();
            _guildStickers = new SynchronizedDictionary<(ulong id, ulong guildId), IEnumerable<byte>>();
            _messages = new SynchronizedDictionary<ulong, SynchronizedDictionary<ulong, IEnumerable<byte>>>();
            _orderedMessages = new SynchronizedDictionary<ulong, ConcurrentQueue<ulong>>();
        }

        #region Methods
        public virtual void CreateChannel(ulong id, IEnumerable<byte> entity)
        {
            _channels.TryAdd(id, entity);
        }
        public virtual void CreateEmote(ulong id, ulong guildId, IEnumerable<byte> entity)
        {
            _guildEmotes.TryAdd((id, guildId), entity);
        }
        public virtual void CreateGuild(ulong id, IEnumerable<byte> entity)
        {
            _guilds.TryAdd(id, entity);

        }
        public virtual void CreateGuildChannel(ulong id, ulong guildId, IEnumerable<byte> entity)
        {
            _guildChannels.TryAdd((id, guildId), entity);

        }
        public virtual void CreateGuildUser(ulong id, ulong guildId, IEnumerable<byte> entity)
        {
            _guildUsers.TryAdd((id, guildId), entity);

        }
        public virtual void CreateMessage(ulong messageId, ulong channelId, IEnumerable<byte> entity)
        {
            if (_messages.ContainsKey(channelId))
            {
                _messages[channelId].TryAdd(messageId, entity);

                if (_orderedMessages.ContainsKey(messageId))
                {
                    while (_orderedMessages[channelId].Count > _messageCacheSize && _orderedMessages[channelId].TryDequeue(out ulong msgId))
                        _messages[channelId].TryRemove(msgId, out _);
                }
                else
                    _orderedMessages[channelId] = new ConcurrentQueue<ulong>();
            }
            else
            {
                _messages[channelId] = new SynchronizedDictionary<ulong, IEnumerable<byte>>(_messageCacheSize);
                _messages[channelId].TryAdd(messageId, entity);
            }


        }
        public virtual void CreateRole(ulong id, ulong guildId, IEnumerable<byte> entity)
        {
            _guildRoles.TryAdd((id, guildId), entity);

        }
        public virtual void CreateSticker(ulong id, ulong guildId, IEnumerable<byte> entity)
        {
            _guildStickers.TryAdd((id, guildId), entity);
        }

        public void CreateThreadMember(ulong id, ulong guildId, ulong threadId, IEnumerable<byte> entity)
        {
            _threadUsers.TryAdd((id, guildId, threadId), entity);
        }

        public virtual void CreateUser(ulong id, IEnumerable<byte> entity)
        {
            _users.TryAdd(id, entity);

        }
        public virtual IEnumerable<byte> DeleteChannel(ulong id)
        {
            if (_channels.TryRemove(id, out var old))
                return old;

            return null;
        }
        public virtual IEnumerable<byte> DeleteEmote(ulong id, ulong guildId)
        {
            if (_guildEmotes.TryRemove((id, guildId), out var old))
                return old;

            return null;
        }
        public virtual IEnumerable<byte> DeleteGuild(ulong id)
        {
            if (_guilds.TryRemove(id, out var old))
                return old;

            return null;
        }
        public virtual IEnumerable<byte> DeleteGuildChannel(ulong id, ulong guildId)
        {
            if (_guildChannels.TryRemove((id, guildId), out var old))
                return old;

            return null;
        }
        public virtual IEnumerable<byte> DeleteGuildUser(ulong id, ulong guildId)
        {
            if (_guildUsers.TryRemove((id, guildId), out var old))
                return old;

            return null;
        }
        public virtual IEnumerable<byte> DeleteMessage(ulong messageId, ulong channelId)
        {
            if (_messages.ContainsKey(channelId))
                if (_messages[channelId].TryRemove(messageId, out var old))
                    return old;

            return null;
        }
        public virtual IEnumerable<byte> DeleteRole(ulong id, ulong guildId)
        {
            if (_guildRoles.TryRemove((id, guildId), out var old))
                return old;

            return null;
        }
        public virtual IEnumerable<byte> DeleteSticker(ulong id, ulong guildId)
        {
            if (_guildStickers.TryRemove((id, guildId), out var old))
                return old;

            return null;
        }

        public IEnumerable<byte> DeleteThreadMember(ulong id, ulong guildId, ulong threadId)
        {
            if (_threadUsers.TryRemove((id, guildId, threadId), out var old))
                return old;
            return null;
        }

        public virtual IEnumerable<byte> DeleteUser(ulong id)
        {
            if (_users.TryRemove(id, out var old))
                return old;

            return null;
        }
        public virtual IEnumerable<byte> GetChannel(ulong id)
        {
            if (_channels.TryGetValue(id, out var entity))
                return entity;
            return null;
        }
        public virtual IEnumerable<byte> GetEmote(ulong id, ulong guildId)
        {
            if (_guildEmotes.TryGetValue((id, guildId), out var entity))
                return entity;
            return null;
        }
        public virtual IEnumerable<IEnumerable<byte>> GetEmotes(ulong guildId)
        {
            return _guildEmotes.Where(x => x.Key.guildId == guildId).Select(x => x.Value);
        }
        public virtual IEnumerable<byte> GetGuild(ulong id)
        {
            if (_guilds.TryGetValue(id, out var entity))
                return entity;
            return null;
        }
        public virtual IEnumerable<byte> GetGuildChannel(ulong id, ulong guildId)
        {
            if (_guildChannels.TryGetValue((id, guildId), out var entity))
                return entity;
            return null;
        }
        public virtual IEnumerable<IEnumerable<byte>> GetGuildChannels(ulong guildId)
        {
            return _guildChannels.Where(x => x.Key.guildId == guildId).Select(x => x.Value);
        }
        public virtual IEnumerable<IEnumerable<byte>> GetGuilds()
        {
            return _guilds.Select(x => x.Value);
        }
        public virtual IEnumerable<byte> GetGuildUser(ulong id, ulong guildId)
        {
            if (_guildUsers.TryGetValue((id, guildId), out var entity))
                return entity;
            return null;
        }
        public virtual IEnumerable<IEnumerable<byte>> GetGuildUsers(ulong guildId)
        {
            return _guildUsers.Where(x => x.Key.guildId == guildId).Select(x => x.Value);
        }
        public virtual IEnumerable<byte> GetMessage(ulong messageId, ulong channelId)
        {
            if (_messages.ContainsKey(channelId))
                if (_messages[channelId].TryGetValue(messageId, out var entity))
                    return entity;
            return null;
        }
        public virtual IEnumerable<IEnumerable<byte>> GetMessages(ulong from, ulong to, ulong channelId)
        {
            if (_messages.ContainsKey(channelId))
            {
                var messages = _messages[channelId].Where(x => x.Key > from && x.Key < to).Select(x => x.Value);

                return messages;
            }

            return new byte[0][];
        }
        public virtual IEnumerable<IEnumerable<byte>> GetMessages(ulong from, Direction dir, ulong channelId, int limit = DiscordConfig.MaxMessagesPerBatch)
        {
            if (_messages.ContainsKey(channelId))
            {
                var messages = _messages[channelId].OrderBy(x => x.Key);

                switch (dir)
                {
                    case Direction.After:
                        return messages.Where(x => x.Key > from).Take(limit).Select(x => x.Value);
                    case Direction.Before:
                        return messages.Where(x => x.Key < from).Take(limit).Select(x => x.Value);
                    case Direction.Around:
                        var l = limit / 2;
                        var a1 = messages.Where(x => x.Key > from).Take(l).Select(x => x.Value);
                        var a2 = messages.Where(x => x.Key < from).Take(l).Select(x => x.Value);
                        return a1.Concat(a2);

                    default:
                        return null;
                }
            }

            return new byte[0][];
        }
        public virtual IEnumerable<byte> GetRole(ulong id, ulong guildId)
        {
            if (_guildRoles.TryGetValue((id, guildId), out var entity))
                return entity;
            return null;
        }
        public virtual IEnumerable<IEnumerable<byte>> GetRoles(ulong guildId)
        {
            return _guildRoles.Where(x => x.Key.guildId == guildId).Select(x => x.Value);
        }
        public virtual IEnumerable<byte> GetSticker(ulong id, ulong guildId)
        {
            if (_guildStickers.TryGetValue((id, guildId), out var entity))
                return entity;
            return null;
        }
        public virtual IEnumerable<IEnumerable<byte>> GetStickers(ulong guildId)
        {
            return _guildStickers.Where(x => x.Key.guildId == guildId).Select(x => x.Value);
        }

        public IEnumerable<byte> GetThreadMember(ulong id, ulong guildId, ulong threadId)
        {
            if (_threadUsers.TryGetValue((id, guildId, threadId), out var entity))
                return entity;
            return null;
        }
        public IEnumerable<IEnumerable<byte>> GetThreadMember(ulong guildId, ulong threadId)
        {
            return _threadUsers.Where(x => x.Key.guildId == guildId && x.Key.threadId == threadId).Select(x => x.Value);
        }

        public virtual IEnumerable<byte> GetUser(ulong id)
        {
            if (_users.TryGetValue(id, out var entity))
                return entity;
            return null;
        }
        public virtual IEnumerable<IEnumerable<byte>> GetUsers()
        {
            return _users.Select(x => x.Value);
        }
        public virtual void UpdateChannel(ulong id, IEnumerable<byte> entity)
        {
            _channels.TryUpdate(id, entity, out var _);

        }
        public virtual void UpdateEmote(ulong id, ulong guildId, IEnumerable<byte> entity)
        {
            _guildEmotes.TryUpdate((id, guildId), entity, out var _);

        }
        public virtual void UpdateGuild(ulong id, IEnumerable<byte> entity)
        {
            _guilds.TryUpdate(id, entity, out var _);

        }
        public virtual void UpdateGuildChannel(ulong id, ulong guildId, IEnumerable<byte> entity)
        {
            _guildChannels.TryUpdate((id, guildId), entity, out var _);

        }
        public virtual void UpdateGuildUser(ulong id, ulong guildId, IEnumerable<byte> entity)
        {
            _guildUsers.TryUpdate((id, guildId), entity, out var _);

        }
        public virtual void UpdateMessage(ulong messageId, ulong channelId, IEnumerable<byte> entity)
        {
            if(_messages.ContainsKey(channelId))
                _messages[channelId].TryUpdate(messageId, entity, out var _);

        }
        public virtual void UpdateRole(ulong id, ulong guildId, IEnumerable<byte> entity)
        {
            _guildRoles.TryUpdate((id, guildId), entity, out var _);

        }
        public virtual void UpdateSticker(ulong id, ulong guildId, IEnumerable<byte> entity)
        {
            _guildStickers.TryUpdate((id, guildId), entity, out var _);

        }

        public void UpdateThreadMember(ulong id, ulong guildId, ulong threadId, IEnumerable<byte> entity)
        {
            _threadUsers.TryUpdate((id, guildId, threadId), entity, out var _);
        }

        public virtual void UpdateUser(ulong id, IEnumerable<byte> entity)
        {
            _users.TryUpdate(id, entity, out var _);

        }
        #endregion
    }
}
