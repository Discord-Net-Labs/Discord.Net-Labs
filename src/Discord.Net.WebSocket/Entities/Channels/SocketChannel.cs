using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based channel.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public abstract class SocketChannel : SocketEntity<ulong>, IChannel, ICacheableEntity<Model, SocketChannel>
    {
        #region SocketChannel
        /// <summary>
        ///     Gets when the channel is created.
        /// </summary>
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
        /// <summary>
        ///     Gets a collection of users from the WebSocket cache.
        /// </summary>
        public IReadOnlyCollection<SocketUser> Users => GetUsersInternal();

        private ChannelType Type;

        internal SocketChannel(DiscordSocketClient discord, ulong id, ChannelType type)
            : base(discord, id)
        {
            this.Type = type;
        }

        /// <exception cref="InvalidOperationException">Unexpected channel type is created.</exception>
        internal static ISocketPrivateChannel CreatePrivate(DiscordSocketClient discord, ClientState state, Model model)
        {
            switch (model.Type)
            {
                case ChannelType.DM:
                    return SocketDMChannel.Create(discord, state, model);
                case ChannelType.Group:
                    return SocketGroupChannel.Create(discord, state, model);
                default:
                    throw new InvalidOperationException($"Unexpected channel type: {model.Type}");
            }
        }
        internal abstract void Update(ClientState state, Model model);
        #endregion

        #region User
        /// <summary>
        ///     Gets a generic user from this channel.
        /// </summary>
        /// <param name="id">The snowflake identifier of the user.</param>
        /// <returns>
        ///     A generic WebSocket-based user associated with the snowflake identifier.
        /// </returns>
        public SocketUser GetUser(ulong id) => GetUserInternal(id);
        internal abstract SocketUser GetUserInternal(ulong id);
        internal abstract IReadOnlyCollection<SocketUser> GetUsersInternal();

        private string DebuggerDisplay => $"Unknown ({Id}, Channel)";
        internal SocketChannel Clone() => MemberwiseClone() as SocketChannel;
        #endregion

        #region IChannel
        /// <inheritdoc />
        string IChannel.Name => null;

        /// <inheritdoc />
        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IUser>(null); //Overridden
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => AsyncEnumerable.Empty<IReadOnlyCollection<IUser>>(); //Overridden
        #endregion

        #region Cache
        internal virtual Model ToCacheable()
        {
            return new Model()
            {
                Id = this.Id,
                Type = this.Type
            };
        }

        Model ICacheableEntity<Model, SocketChannel>.ToCacheable() => ToCacheable();
        #endregion
    }
}
