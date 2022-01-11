using System;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents an interaction type for Message Components.
    /// </summary>
    public interface IComponentInteraction : IDiscordInteraction
    {
        /// <summary>
        ///     Gets the data received with this interaction, contains the button that was clicked.
        /// </summary>
        new IComponentInteractionData Data { get; }

        /// <summary>
        ///     Gets the message that contained the trigger for this interaction.
        /// </summary>
        IUserMessage Message { get; }

        /// <summary>
        ///     Updates the message which this component resides in with the type <see cref="InteractionResponseType.UpdateMessage"/>
        /// </summary>
        /// <param name="func">A delegate containing the properties to modify the message with.</param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        /// <returns>A task that represents the asynchronous operation of updating the message.</returns>
        Task UpdateAsync(Action<MessageProperties> func, RequestOptions options = null);
    }
}
