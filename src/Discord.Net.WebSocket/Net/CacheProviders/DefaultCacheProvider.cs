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
        public virtual ValueTask CreateChannelAsync(ulong id, IEnumerable<byte> entity)
        {
            _channels.TryAdd(id, entity);
            return new ValueTask(Task.CompletedTask);
        }
        public virtual ValueTask CreateEmoteAsync(ulong id, ulong guildId, IEnumerable<byte> entity)
        {
            _guildEmotes.TryAdd((id, guildId), entity);
            return new ValueTask(Task.CompletedTask);
        }
        public virtual ValueTask CreateGuildAsync(ulong id, IEnumerable<byte> entity)
        {
            _guilds.TryAdd(id, entity);
            return new ValueTask(Task.CompletedTask);
        }
        public virtual ValueTask CreateGuildChannelAsync(ulong id, ulong guildId, IEnumerable<byte> entity)
        {
            _guildChannels.TryAdd((id, guildId), entity);
            return new ValueTask(Task.CompletedTask);
        }
        public virtual ValueTask CreateGuildUserAsync(ulong id, ulong guildId, IEnumerable<byte> entity)
        {
            _guildUsers.TryAdd((id, guildId), entity);
            return new ValueTask(Task.CompletedTask);
        }
        public virtual ValueTask CreateMessageAsync(ulong messageId, ulong channelId, IEnumerable<byte> entity)
        {
            if(_messages.ContainsKey(channelId))
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

            return new ValueTask(Task.CompletedTask);
        }
        public virtual ValueTask CreateRoleAsync(ulong id, ulong guildId, IEnumerable<byte> entity)
        {
            _guildRoles.TryAdd((id, guildId), entity);
            return new ValueTask(Task.CompletedTask);
        }
        public virtual ValueTask CreateStickerAsync(ulong id, ulong guildId, IEnumerable<byte> entity)
        {
            _guildStickers.TryAdd((id, guildId), entity);
            return new ValueTask(Task.CompletedTask);
        }
        public virtual ValueTask CreateUserAsync(ulong id, IEnumerable<byte> entity)
        {
            _users.TryAdd(id, entity);
            return new ValueTask(Task.CompletedTask);
        }
        public virtual ValueTask<IEnumerable<byte>> DeleteChannelAsync(ulong id)
        {
            if (_channels.TryRemove(id, out var old))
                return new ValueTask<IEnumerable<byte>>(old);

            return new ValueTask<IEnumerable<byte>>(result: null);
        }
        public virtual ValueTask<IEnumerable<byte>> DeleteEmoteAsync(ulong id, ulong guildId)
        {
            if (_guildEmotes.TryRemove((id, guildId), out var old))
                return new ValueTask<IEnumerable<byte>>(old);

            return new ValueTask<IEnumerable<byte>>(result: null);
        }
        public virtual ValueTask<IEnumerable<byte>> DeleteGuildAsync(ulong id)
        {
            if (_guilds.TryRemove(id, out var old))
                return new ValueTask<IEnumerable<byte>>(old);

            return new ValueTask<IEnumerable<byte>>(result: null);
        }
        public virtual ValueTask<IEnumerable<byte>> DeleteGuildChannelAsync(ulong id, ulong guildId)
        {
            if (_guildChannels.TryRemove((id, guildId), out var old))
                return new ValueTask<IEnumerable<byte>>(old);

            return new ValueTask<IEnumerable<byte>>(result: null);
        }
        public virtual ValueTask<IEnumerable<byte>> DeleteGuildUserAsync(ulong id, ulong guildId)
        {
            if (_guildUsers.TryRemove((id, guildId), out var old))
                return new ValueTask<IEnumerable<byte>>(old);

            return new ValueTask<IEnumerable<byte>>(result: null);
        }
        public virtual ValueTask<IEnumerable<byte>> DeleteMessageAsync(ulong messageId, ulong channelId)
        {
            if(_messages.ContainsKey(channelId))
                if (_messages[channelId].TryRemove(messageId, out var old))
                    return new ValueTask<IEnumerable<byte>>(old);

            return new ValueTask<IEnumerable<byte>>(result: null);
        }
        public virtual ValueTask<IEnumerable<byte>> DeleteRoleAsync(ulong id, ulong guildId)
        {
            if (_guildRoles.TryRemove((id, guildId), out var old))
                return new ValueTask<IEnumerable<byte>>(old);

            return new ValueTask<IEnumerable<byte>>(result: null);
        }
        public virtual ValueTask<IEnumerable<byte>> DeleteStickerAsync(ulong id, ulong guildId)
        {
            if (_guildStickers.TryRemove((id, guildId), out var old))
                return new ValueTask<IEnumerable<byte>>(old);

            return new ValueTask<IEnumerable<byte>>(result: null);
        }
        public virtual ValueTask<IEnumerable<byte>> DeleteUserAsync(ulong id)
        {
            if (_users.TryRemove(id, out var old))
                return new ValueTask<IEnumerable<byte>>(old);

            return new ValueTask<IEnumerable<byte>>(result: null);
        }
        public virtual ValueTask<IEnumerable<byte>> GetChannelAsync(ulong id)
        {
            if (_channels.TryGetValue(id, out var entity))
                return new ValueTask<IEnumerable<byte>>(entity);
            return new ValueTask<IEnumerable<byte>>(result: null);
        }
        public virtual ValueTask<IEnumerable<byte>> GetEmoteAsync(ulong id, ulong guildId)
        {
            if (_guildEmotes.TryGetValue((id, guildId), out var entity))
                return new ValueTask<IEnumerable<byte>>(entity);
            return new ValueTask<IEnumerable<byte>>(result: null);
        }
        public virtual ValueTask<IEnumerable<IEnumerable<byte>>> GetEmotesAsync(ulong guildId)
        {
            return new ValueTask<IEnumerable<IEnumerable<byte>>>(result: _guildEmotes.Where(x => x.Key.guildId == guildId).Select(x => x.Value));
        }
        public virtual ValueTask<IEnumerable<byte>> GetGuildAsync(ulong id)
        {
            if (_guilds.TryGetValue(id, out var entity))
                return new ValueTask<IEnumerable<byte>>(entity);
            return new ValueTask<IEnumerable<byte>>(result: null);
        }
        public virtual ValueTask<IEnumerable<byte>> GetGuildChannelAsync(ulong id, ulong guildId)
        {
            if (_guildChannels.TryGetValue((id, guildId), out var entity))
                return new ValueTask<IEnumerable<byte>>(entity);
            return new ValueTask<IEnumerable<byte>>(result: null);
        }
        public virtual ValueTask<IEnumerable<IEnumerable<byte>>> GetGuildChannelsAsync(ulong guildId)
        {
            return new ValueTask<IEnumerable<IEnumerable<byte>>>(result: _guildChannels.Where(x => x.Key.guildId == guildId).Select(x => x.Value));
        }
        public virtual ValueTask<IEnumerable<IEnumerable<byte>>> GetGuildsAsync()
        {
            return new ValueTask<IEnumerable<IEnumerable<byte>>>(result: _guilds.Select(x => x.Value));
        }
        public virtual ValueTask<IEnumerable<byte>> GetGuildUserAsync(ulong id, ulong guildId)
        {
            if (_guildUsers.TryGetValue((id, guildId), out var entity))
                return new ValueTask<IEnumerable<byte>>(entity);
            return new ValueTask<IEnumerable<byte>>(result: null);
        }
        public virtual ValueTask<IEnumerable<IEnumerable<byte>>> GetGuildUsersAsync(ulong guildId)
        {
            return new ValueTask<IEnumerable<IEnumerable<byte>>>(result: _guildUsers.Where(x => x.Key.guildId == guildId).Select(x => x.Value));
        }
        public virtual ValueTask<IEnumerable<byte>> GetMessageAsync(ulong messageId, ulong channelId)
        {
            if(_messages.ContainsKey(channelId))
                if (_messages[channelId].TryGetValue(messageId, out var entity))
                    return new ValueTask<IEnumerable<byte>>(entity);
            return new ValueTask<IEnumerable<byte>>(result: null);
        }
        public virtual ValueTask<IEnumerable<IEnumerable<byte>>> GetMessagesAsync(ulong from, ulong to, ulong channelId)
        {
            if (_messages.ContainsKey(channelId))
            {
                var messages = _messages[channelId].Where(x => x.Key > from && x.Key < to).Select(x => x.Value);

                return new ValueTask<IEnumerable<IEnumerable<byte>>>(messages);
            }

            return new ValueTask<IEnumerable<IEnumerable<byte>>>(new byte[0][]);
        }
        public virtual ValueTask<IEnumerable<IEnumerable<byte>>> GetMessagesAsync(ulong from, Direction dir, ulong channelId, int limit = DiscordConfig.MaxMessagesPerBatch)
        {
            if (_messages.ContainsKey(channelId))
            {
                var messages = _messages[channelId].OrderBy(x => x.Key);

                switch (dir)
                {
                    case Direction.After:
                        return new ValueTask<IEnumerable<IEnumerable<byte>>>(messages.Where(x => x.Key > from).Take(limit).Select(x => x.Value));
                    case Direction.Before:
                        return new ValueTask<IEnumerable<IEnumerable<byte>>>(messages.Where(x => x.Key < from).Take(limit).Select(x => x.Value));
                    case Direction.Around:
                        var l = limit / 2;
                        var a1 = messages.Where(x => x.Key > from).Take(l).Select(x => x.Value);
                        var a2 = messages.Where(x => x.Key < from).Take(l).Select(x => x.Value);
                        return new ValueTask<IEnumerable<IEnumerable<byte>>>(a1.Concat(a2));

                    default:
                        return new ValueTask<IEnumerable<IEnumerable<byte>>>(result: null);
                }
            }

            return new ValueTask<IEnumerable<IEnumerable<byte>>>(new byte[0][]);
        }
        public virtual ValueTask<IEnumerable<byte>> GetRoleAsync(ulong id, ulong guildId)
        {
            if (_guildRoles.TryGetValue((id, guildId), out var entity))
                return new ValueTask<IEnumerable<byte>>(entity);
            return new ValueTask<IEnumerable<byte>>(result: null);
        }
        public virtual ValueTask<IEnumerable<IEnumerable<byte>>> GetRolesAsync(ulong guildId)
        {
            return new ValueTask<IEnumerable<IEnumerable<byte>>>(_guildRoles.Where(x => x.Key.guildId == guildId).Select(x => x.Value));
        }
        public virtual ValueTask<IEnumerable<byte>> GetStickerAsync(ulong id, ulong guildId)
        {
            if (_guildStickers.TryGetValue((id, guildId), out var entity))
                return new ValueTask<IEnumerable<byte>>(entity);
            return new ValueTask<IEnumerable<byte>>(result: null);
        }
        public virtual ValueTask<IEnumerable<IEnumerable<byte>>> GetStickersAsync(ulong guildId)
        {
            return new ValueTask<IEnumerable<IEnumerable<byte>>>(_guildStickers.Where(x => x.Key.guildId == guildId).Select(x => x.Value));
        }
        public virtual ValueTask<IEnumerable<byte>> GetUserAsync(ulong id)
        {
            if (_users.TryGetValue(id, out var entity))
                return new ValueTask<IEnumerable<byte>>(entity);
            return new ValueTask<IEnumerable<byte>>(result: null);
        }
        public virtual ValueTask<IEnumerable<IEnumerable<byte>>> GetUsersAsync()
        {
            return new ValueTask<IEnumerable<IEnumerable<byte>>>(_users.Select(x => x.Value));
        }
        public virtual ValueTask UpdateChannelAsync(ulong id, IEnumerable<byte> entity)
        {
            _channels.TryUpdate(id, entity, out var _);
            return new ValueTask(Task.CompletedTask);
        }
        public virtual ValueTask UpdateEmoteAsync(ulong id, ulong guildId, IEnumerable<byte> entity)
        {
            _guildEmotes.TryUpdate((id, guildId), entity, out var _);
            return new ValueTask(Task.CompletedTask);
        }
        public virtual ValueTask UpdateGuildAsync(ulong id, IEnumerable<byte> entity)
        {
            _guilds.TryUpdate(id, entity, out var _);
            return new ValueTask(Task.CompletedTask);
        }
        public virtual ValueTask UpdateGuildChannelAsync(ulong id, ulong guildId, IEnumerable<byte> entity)
        {
            _guildChannels.TryUpdate((id, guildId), entity, out var _);
            return new ValueTask(Task.CompletedTask);
        }
        public virtual ValueTask UpdateGuildUserAsync(ulong id, ulong guildId, IEnumerable<byte> entity)
        {
            _guildUsers.TryUpdate((id, guildId), entity, out var _);
            return new ValueTask(Task.CompletedTask);
        }
        public virtual ValueTask UpdateMessageAsync(ulong messageId, ulong channelId, IEnumerable<byte> entity)
        {
            if(_messages.ContainsKey(channelId))
                _messages[channelId].TryUpdate(messageId, entity, out var _);
            return new ValueTask(Task.CompletedTask);
        }
        public virtual ValueTask UpdateRoleAsync(ulong id, ulong guildId, IEnumerable<byte> entity)
        {
            _guildRoles.TryUpdate((id, guildId), entity, out var _);
            return new ValueTask(Task.CompletedTask);
        }
        public virtual ValueTask UpdateStickerAsync(ulong id, ulong guildId, IEnumerable<byte> entity)
        {
            _guildStickers.TryUpdate((id, guildId), entity, out var _);
            return new ValueTask(Task.CompletedTask);
        }
        public virtual ValueTask UpdateUserAsync(ulong id, IEnumerable<byte> entity)
        {
            _users.TryUpdate(id, entity, out var _);
            return new ValueTask(Task.CompletedTask);
        }
        #endregion
    }
}
