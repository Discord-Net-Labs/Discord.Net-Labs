using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    internal class ClientState
    {
        private readonly ICacheProvider cacheProvider;

        internal IReadOnlyCollection<SocketChannel> Channels => _channels.ToReadOnlyCollection();
        internal IReadOnlyCollection<SocketDMChannel> DMChannels => _dmChannels.ToReadOnlyCollection();
        internal IReadOnlyCollection<SocketGroupChannel> GroupChannels => _groupChannels.Select(x => GetChannel(x) as SocketGroupChannel).ToReadOnlyCollection(_groupChannels);
        internal IReadOnlyCollection<SocketGuild> Guilds => _guilds.ToReadOnlyCollection();
        internal IReadOnlyCollection<SocketGlobalUser> Users => _users.ToReadOnlyCollection();

        internal IReadOnlyCollection<ISocketPrivateChannel> PrivateChannels =>
            _dmChannels.Select(x => x.Value as ISocketPrivateChannel).Concat(
                _groupChannels.Select(x => GetChannel(x) as ISocketPrivateChannel))
            .ToReadOnlyCollection(() => _dmChannels.Count + _groupChannels.Count);

        public ClientState(ICacheProvider provider)
        {
            cacheProvider = provider;
        }

        internal async Task<SocketChannel> GetChannel(ulong id)
        {
            var data = await cacheProvider.GetChannelAsync(id).ConfigureAwait(false);

            return EntityConverter.Decode<SocketChannel>(data) as SocketChannel;
        }
        internal async Task<SocketDMChannel> GetDMChannel(ulong userId)
        {
            if (_dmChannels.TryGetValue(userId, out SocketDMChannel channel))
                return channel;
            return null;
        }
        internal void AddChannel(SocketChannel channel)
        {
            _channels[channel.Id] = channel;

            switch (channel)
            {
                case SocketDMChannel dmChannel:
                    _dmChannels[dmChannel.Recipient.Id] = dmChannel;
                    break;
                case SocketGroupChannel groupChannel:
                    _groupChannels.TryAdd(groupChannel.Id);
                    break;
            }
        }
        internal SocketChannel RemoveChannel(ulong id)
        {
            if (_channels.TryRemove(id, out SocketChannel channel))
            {
                switch (channel)
                {
                    case SocketDMChannel dmChannel:
                        _dmChannels.TryRemove(dmChannel.Recipient.Id, out _);
                        break;
                    case SocketGroupChannel _:
                        _groupChannels.TryRemove(id);
                        break;
                }
                return channel;
            }
            return null;
        }
        internal void PurgeAllChannels()
        {
            foreach (var guild in _guilds.Values)
                guild.PurgeChannelCache(this);

            PurgeDMChannels();
        }
        internal void PurgeDMChannels()
        {
            foreach (var channel in _dmChannels.Values)
                _channels.TryRemove(channel.Id, out _);

            _dmChannels.Clear();
        }

        internal SocketGuild GetGuild(ulong id)
        {
            if (_guilds.TryGetValue(id, out SocketGuild guild))
                return guild;
            return null;
        }
        internal void AddGuild(SocketGuild guild)
        {
            _guilds[guild.Id] = guild;
        }
        internal SocketGuild RemoveGuild(ulong id)
        {
            if (_guilds.TryRemove(id, out SocketGuild guild))
            {
                guild.PurgeChannelCache(this);
                guild.PurgeGuildUserCache();
                return guild;
            }
            return null;
        }

        internal SocketGlobalUser GetUser(ulong id)
        {
            if (_users.TryGetValue(id, out SocketGlobalUser user))
                return user;
            return null;
        }
        internal SocketGlobalUser GetOrAddUser(ulong id, Func<ulong, SocketGlobalUser> userFactory)
        {
            return _users.GetOrAdd(id, userFactory);
        }
        internal SocketGlobalUser RemoveUser(ulong id)
        {
            if (_users.TryRemove(id, out SocketGlobalUser user))
                return user;
            return null;
        }
        internal void PurgeUsers()
        {
            foreach (var guild in _guilds.Values)
                guild.PurgeGuildUserCache();
        }

        internal SocketApplicationCommand GetCommand(ulong id)
        {
            if (_commands.TryGetValue(id, out SocketApplicationCommand command))
                return command;
            return null;
        }
        internal void AddCommand(SocketApplicationCommand command)
        {
            _commands[command.Id] = command;
        }
        internal SocketApplicationCommand GetOrAddCommand(ulong id, Func<ulong, SocketApplicationCommand> commandFactory)
        {
            return _commands.GetOrAdd(id, commandFactory);
        }
        internal SocketApplicationCommand RemoveCommand(ulong id)
        {
            if (_commands.TryRemove(id, out SocketApplicationCommand command))
                return command;
            return null;
        }
        internal void PurgeCommands(Func<SocketApplicationCommand, bool> precondition)
        {
            var ids = _commands.Where(x => precondition(x.Value)).Select(x => x.Key);

            foreach (var id in ids)
                _commands.TryRemove(id, out var _);
        }
    }
}
