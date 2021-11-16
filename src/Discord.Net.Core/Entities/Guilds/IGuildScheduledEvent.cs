using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IGuildScheduledEvent : IEntity<ulong>
    {
        /// <summary>
        ///     Gets the guild this event is scheduled in.
        /// </summary>
        IGuild Guild { get; }

        /// <summary>
        ///     Gets the optional channel id where this event will be hosted.
        /// </summary>
        ulong? ChannelId { get; }

        /// <summary>
        ///     Gets the user who created the event.
        /// </summary>
        IUser Creator { get; }

        /// <summary>
        ///     Gets the name of the event.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the description of the event.
        /// </summary>
        /// <remarks>
        ///     This field is <see langword="null"/> when the event doesn't have a discription.
        /// </remarks>
        string Description { get; }

        /// <summary>
        ///     Gets the start time of the event.
        /// </summary>
        DateTimeOffset StartTime { get; }

        /// <summary>
        ///     Gets the optional end time of the event.
        /// </summary>
        DateTimeOffset? EndTime { get; }

        /// <summary>
        ///     Gets the privacy level of the event.
        /// </summary>
        GuildScheduledEventPrivacyLevel PrivacyLevel { get; }

        /// <summary>
        ///     Gets the status of the event.
        /// </summary>
        GuildScheduledEventStatus Status { get; }

        /// <summary>
        ///     Gets the type of the event.
        /// </summary>
        GuildScheduledEventType Type { get; }

        /// <summary>
        ///     Gets the optional entity id of the event. The "entity" of the event
        ///     can be a stage instance event as is seperate from <see cref="ChannelId"/>.
        /// </summary>
        ulong? EntityId { get; }

        /// <summary>
        ///     Gets a collection of speakers for the event.
        /// </summary>
        IReadOnlyCollection<ulong> Speakers { get; }

        /// <summary>
        ///     Gets the location of the event if the <see cref="Type"/> is external.
        /// </summary>
        string Location { get; }

        /// <summary>
        ///     Gets the user count of the event.
        /// </summary>
        int? UserCount { get; }

        /// <summary>
        ///     Modifies the guild event.
        /// </summary>
        /// <param name="func">The delegate containing the properties to modify the event with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        Task ModifyAsync(Action<GuildScheduledEventsProperties> func, RequestOptions options = null);

        /// <summary>
        ///     Deletes the current event.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous delete operation.
        /// </returns>
        Task DeleteAsync(RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of users that are interested in this event.
        /// </summary>
        /// <param name="limit">The amount of users to fetch.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A read-only collection of users.
        /// </returns>
        Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync(int limit = 100, RequestOptions options = null);
    }
}
