using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic user.
    /// </summary>
    public interface IUser : ISnowflakeEntity, IMentionable, IPresence
    {
        /// <summary>
        ///     Gets the identifier of this user's avatar.
        /// </summary>
        string AvatarId { get; }
        /// <summary>
        ///     Gets the identifier of this user's banner.
        /// </summary>
        string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128);
        /// <summary>
        ///     Gets the banner URL for this user.
        /// </summary>
        /// <param name="format">The format to return.</param>
        /// <param name="size">The size of the image to return in. This can be any power of two between 16 and 2048.
        /// </param>
        /// <returns>
        ///     A string representing the user's avatar URL; <c>null</c> if the user does not have an banner in place.
        /// </returns>
        string GetDefaultAvatarUrl();
        /// <summary>
        ///     Gets the per-username unique ID for this user.
        /// </summary>
        string Discriminator { get; }
        /// <summary>
        ///     Gets the per-username unique ID for this user.
        /// </summary>
        ushort DiscriminatorValue { get; }
        /// <summary>
        ///     Gets a value that indicates whether this user is identified as a bot.
        /// </summary>
        /// <remarks>
        ///     This property retrieves a value that indicates whether this user is a registered bot application
        ///     (indicated by the blue BOT tag within the official chat client).
        /// </remarks>
        /// <returns>
        ///     <c>true</c> if the user is a bot application; otherwise <c>false</c>.
        /// </returns>
        bool IsBot { get; }
        /// <summary>
        ///     Gets a value that indicates whether this user is a webhook user.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the user is a webhook; otherwise <c>false</c>.
        /// </returns>
        bool IsWebhook { get; }
        /// <summary>
        ///     Gets the username for this user.
        /// </summary>
        string Username { get; }
        /// <summary>
        ///     Gets the public flags that are applied to this user's account.
        /// </summary>
        /// <remarks>
        ///     This value is determined by bitwise OR-ing <see cref="UserProperties"/> values together.
        /// </remarks>
        /// <returns>
        ///     The value of public flags for this user.
        /// </returns>
        UserProperties? PublicFlags { get; }

        /// <summary>
        ///     Creates the direct message channel of this user.
        /// </summary>
        /// <remarks>
        ///     This method is used to obtain or create a channel used to send a direct message.
        ///     <note type="warning">
        ///          In event that the current user cannot send a message to the target user, a channel can and will
        ///          still be created by Discord. However, attempting to send a message will yield a
        ///          <see cref="Discord.Net.HttpException"/> with a 403 as its
        ///          <see cref="Discord.Net.HttpException.HttpCode"/>. There are currently no official workarounds by
        ///          Discord.
        ///     </note>
        /// </remarks>
        /// <example>
        ///     <para>The following example attempts to send a direct message to the target user and logs the incident should
        ///     it fail.</para>
        ///     <code region="CreateDMChannelAsync" language="cs"
        ///           source="../../../Discord.Net.Examples/Core/Entities/Users/IUser.Examples.cs"/>
        /// </example>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation for getting or creating a DM channel. The task result
        ///     contains the DM channel associated with this user.
        /// </returns>
        Task<IDMChannel> CreateDMChannelAsync(RequestOptions options = null);
    }
}
