using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.GuildScheduledEvent;

namespace Discord.Rest
{
    public class RestGuildEvent : RestEntity<ulong>, IGuildScheduledEvent
    {
        /// <inheritdoc/>
        public IGuild Guild { get; private set; }

        /// <inheritdoc/>
        public ulong? ChannelId { get; private set; }

        /// <inheritdoc/>
        public IUser Creator { get; private set; }

        /// <inheritdoc/>
        public ulong CreatorId { get; private set; }

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

        /// <inheritdoc/>
        public IReadOnlyCollection<ulong> Speakers { get; private set; }

        /// <inheritdoc/>
        public string Location { get; private set; }

        /// <inheritdoc/>
        public int? UserCount { get; private set; }

        internal RestGuildEvent(BaseDiscordClient client, IGuild guild, ulong id)
            : base(client, id)
        {
            Guild = guild;
        }

        internal static RestGuildEvent Create(BaseDiscordClient client, IGuild guild, Model model)
        {
            var entity = new RestGuildEvent(client, guild, model.Id);
            entity.Update(model);
            return entity;
        }

        internal static RestGuildEvent Create(BaseDiscordClient client, IGuild guild, IUser creator, Model model)
        {
            var entity = new RestGuildEvent(client, guild, model.Id);
            entity.Update(model, creator);
            return entity;
        }

        internal void Update(Model model, IUser creator)
        {
            Update(model);
            Creator = creator;
            CreatorId = creator.Id;
        }

        internal void Update(Model model)
        {
            if (model.Creator.IsSpecified)
            {
                Creator = RestUser.Create(Discord, model.Creator.Value);
            }

            CreatorId = model.CreatorId.ToNullable() ?? 0; // should be changed?
            ChannelId = model.ChannelId.ToNullable();
            Name = model.Name;
            Description = model.Description.GetValueOrDefault();
            StartTime = model.ScheduledStartTime;
            EndTime = model.ScheduledEndTime;
            PrivacyLevel = model.PrivacyLevel;
            Status = model.Status;
            Type = model.EntityType;
            EntityId = model.EntityId;
            Speakers = model.EntityMetadata?.SpeakerIds.GetValueOrDefault(new ulong[0]).ToImmutableArray() ?? ImmutableArray<ulong>.Empty;
            Location = model.EntityMetadata?.Location.GetValueOrDefault();
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
        async Task<IReadOnlyCollection<IGuildUser>> IGuildScheduledEvent.GetUsersAsync(int limit, RequestOptions options)
            => await GetUsersAsync(limit, options).ConfigureAwait(false);
    }
}
