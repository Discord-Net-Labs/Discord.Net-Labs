using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.GuildScheduledEvent;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based guild event.
    /// </summary>
    public class SocketGuildEvent : SocketEntity<ulong>, IGuildScheduledEvent
    {
        /// <summary>
        ///     Gets the guild of the event.
        /// </summary>
        public SocketGuild Guild { get; private set; }

        /// <summary>
        ///     Gets the channel of the event.
        /// </summary>
        public SocketGuildChannel Channel { get; private set; }

        /// <summary>
        ///     Gets the user who created the event.
        /// </summary>
        public SocketGuildUser Creator { get; private set; }

        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public string Description { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset StartTime { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset? EndTime { get; private set; }

        /// <inheritdoc/>
        public GuildScheduledEventPrivacyLevel PrivacyLevel { get; private set; }

        /// <inheritdoc/>
        public GuildScheduledEventStatus Status { get; private set; }

        /// <inheritdoc/>
        public GuildScheduledEventType Type { get; private set; }

        /// <inheritdoc/>
        public ulong? EntityId { get; private set; }

        /// <summary>
        ///     Gets a collection of speakers in this event.
        /// </summary>
        public IReadOnlyCollection<SocketGuildUser> Speakers { get; private set; }

        /// <inheritdoc/>
        public string Location { get; private set; }

        /// <inheritdoc/>
        public int? UserCount { get; private set; }

        internal SocketGuildEvent(DiscordSocketClient client, SocketGuild guild, ulong id)
            : base(client, id)
        {
            Guild = guild;
        }

        internal static SocketGuildEvent Create(DiscordSocketClient client, SocketGuild guild, Model model)
        {
            var entity = new SocketGuildEvent(client, guild, model.Id);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            if (model.ChannelId.IsSpecified && model.ChannelId.Value != null)
            {
                Channel = Guild.GetChannel(model.ChannelId.Value.Value);
            }

            if (model.CreatorId.IsSpecified)
            {
                var guildUser = Guild.GetUser(model.CreatorId.Value);

                if(guildUser != null)
                {
                    if(model.Creator.IsSpecified)
                        guildUser.Update(Discord.State, model.Creator.Value);

                    Creator = guildUser;
                }
                else if (guildUser == null && model.Creator.IsSpecified)
                {
                    guildUser = SocketGuildUser.Create(Guild, Discord.State, model.Creator.Value);
                    Creator = guildUser;
                }
            }

            Name = model.Name;
            Description = model.Description.GetValueOrDefault();

            EntityId = model.EntityId;
            Location = model.EntityMetadata?.Location.GetValueOrDefault();
            Speakers = model.EntityMetadata?.SpeakerIds.GetValueOrDefault(new ulong[0]).Select(x => Guild.GetUser(x)).Where(x => x != null).ToImmutableArray() ?? ImmutableArray.Create<SocketGuildUser>();
            Type = model.EntityType;

            PrivacyLevel = model.PrivacyLevel;
            EndTime = model.ScheduledEndTime;
            StartTime = model.ScheduledStartTime;
            Status = model.Status;
            UserCount = model.UserCount.ToNullable();
        }

        /// <inheritdoc/>
        public Task DeleteAsync(RequestOptions options = null)
            => GuildHelper.DeleteEventAsync(Discord, this, options);

        /// <inheritdoc/>
        public async Task ModifyAsync(Action<GuildScheduledEventsProperties> func, RequestOptions options = null)
        {
            var model = await GuildHelper.ModifyGuildEventAsync(Discord, func, this, options).ConfigureAwait(false);
            Update(model);
        }

        /// <summary>
        ///     Gets a collection of users that are interested in this event.
        /// </summary>
        /// <param name="limit">The amount of users to fetch.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A read-only collection of users.
        /// </returns>
        public Task<IReadOnlyCollection<RestGuildUser>> GetUsersAsync(int limit = 100, RequestOptions options = null)
            => GuildHelper.GetEventUsersAsync(Discord, this, limit, options);

        /// <inheritdoc/>
        IGuild IGuildScheduledEvent.Guild => Guild;
        /// <inheritdoc/>
        IUser IGuildScheduledEvent.Creator => Creator;
        /// <inheritdoc/>
        ulong? IGuildScheduledEvent.ChannelId => Channel?.Id;
        /// <inheritdoc/>
        IReadOnlyCollection<ulong> IGuildScheduledEvent.Speakers => Speakers.Select(x => x.Id).ToImmutableArray();
        /// <inheritdoc/>
        async Task<IReadOnlyCollection<IGuildUser>> IGuildScheduledEvent.GetUsersAsync(int limit, RequestOptions options)
            => await GetUsersAsync(limit, options);
    }
}
