using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    /// Represents an Discord Client originated user interaction
    /// </summary>
    public interface IDiscordInteraction : IDeletable, ISnowflakeEntity
    {
        /// <summary>
        /// Get the type of the interaction
        /// </summary>
        InteractionType InteractionType { get; }
        /// <summary>
        /// Get the user that created this interaction
        /// </summary>
        IUser User { get; }
        /// <summary>
        /// Get the channel this interaction originated from
        /// </summary>
        IMessageChannel Channel { get; }
        /// <summary>
        /// Get the manipulation token for this interaction
        /// </summary>
        /// <remarks>
        /// Valid for 15 mins
        /// </remarks>
        IDiscordInteractionToken Token { get; }
        /// <summary>
        /// Get the version of the Interaction API
        /// </summary>
        /// <remarks>
        /// Constant 1
        /// </remarks>
        int Version { get; }
        /// <summary>
        /// Get the Snowflake ID of the application this interaction was issued to
        /// </summary>
        internal ulong ApplicationId { get; }

        /// <summary>
        /// Send an acknowledgement to verify the handoff
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task AcknowledgeAsync (RequestOptions options);

        /// <summary>
        /// Send a response that will remove the thinking animation from the original acknowledgement and displayed to the user
        /// </summary>
        /// <param name="text">Text content of the response</param>
        /// <param name="isTTS">Determines whether the message should be read aloud by Discord or not.</param>
        /// <param name="embeds">The embeds to be sent with the response</param>
        /// <param name="allowedMentions">Specifies if notifications are sent for mentioned users and roles in the message <paramref name="text"/>.
        /// If <see langword="null"/>, all mentioned roles and users will be notified.</param>
        /// <param name="flags">Response flags to determine the responses behaviour</param>
        /// <param name="messageComponents">Message components that will be sent with the response</param>
        /// <param name="options">The options to be used when sending the request</param>
        /// <returns></returns>
        Task PopulateAcknowledgement (string text, bool isTTS, IEnumerable<Embed> embeds, AllowedMentions allowedMentions,
            InteractionApplicationCommandCallbackFlags flags, IEnumerable<MessageComponent> messageComponents, RequestOptions options);

        /// <summary>
        /// Send a response that will be directly shown to the user without displaying the "Thinking" animation
        /// </summary>
        /// <remarks>
        /// If this method is preferred over the "Acknowledge, Modify" method, response must be sent right away for the interaction hand-off to be successful,
        /// otherwise an "interction failed" message will be displayed to the user.
        /// </remarks>
        /// <param name="text">Text content of the response</param>
        /// <param name="isTTS">Determines whether the message should be read aloud by Discord or not.</param>
        /// <param name="embeds">The embeds to be sent with the response</param>
        /// <param name="allowedMentions">Specifies if notifications are sent for mentioned users and roles in the message <paramref name="text"/>.
        /// If <see langword="null"/>, all mentioned roles and users will be notified.</param>
        /// <param name="flags">Response flags to determine the responses behaviour</param>
        /// <param name="messageComponents">Message components that will be sent with the response</param>
        /// <param name="options">The options to be used when sending the request</param>
        /// <returns></returns>
        Task SendResponse (string text, bool isTTS, IEnumerable<Embed> embeds, AllowedMentions allowedMentions,
            InteractionApplicationCommandCallbackFlags flags, IEnumerable<MessageComponent> messageComponents, RequestOptions options);

        /// <summary>
        /// Delete the Interaction Response
        /// </summary>
        /// <param name="options">The options to be used when sending the request</param>
        /// <returns></returns>
        new Task DeleteAsync (RequestOptions options);

        /// <summary>
        /// Send a followup message for this interaction
        /// </summary>
        /// <param name="text">Text content of the response</param>
        /// <param name="isTTS">Determines whether the message should be read aloud by Discord or not.</param>
        /// <param name="username">override the default username</param>
        /// <param name="avatarUrl">override the default avatar</param>
        /// <param name="embeds">The embeds to be sent with the response</param>
        /// <param name="allowedMentions">Specifies if notifications are sent for mentioned users and roles in the message <paramref name="text"/>.
        /// If <see langword="null"/>, all mentioned roles and users will be notified.</param>
        /// <param name="options">The options to be used when sending the request</param>
        /// <returns></returns>
        Task<IMessage> SendFollowupAsync (string text, bool isTTS, string username, string avatarUrl, IEnumerable<Embed> embeds,
            AllowedMentions allowedMentions, RequestOptions options);

        /// <summary>
        /// Modify an Interaction Followup message
        /// </summary>
        /// <param name="messageId">Id of the followup message that will be modified</param>
        /// <param name="text">Text content of the response</param>
        /// <param name="embeds">The embeds to be sent with the response</param>
        /// <param name="allowedMentions">Specifies if notifications are sent for mentioned users and roles in the message <paramref name="text"/>.
        /// If <see langword="null"/>, all mentioned roles and users will be notified.</param>
        /// <param name="options">The options to be used when sending the request</param>
        /// <returns></returns>
        Task ModifyFollowup (ulong messageId, string text, IEnumerable<Embed> embeds, AllowedMentions allowedMentions, RequestOptions options);

        /// <summary>
        /// Delete an Interaction Followup message
        /// </summary>
        /// <param name="messageId">Id of the followup message that will be deleted</param>
        /// <param name="options">The options to be used when sending the request</param>
        /// <returns></returns>
        Task DeleteFollowup (ulong messageId, RequestOptions options);
    }
}
