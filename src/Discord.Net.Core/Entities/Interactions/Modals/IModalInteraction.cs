namespace Discord
{
    /// <summary>
    ///     Represents an interaction type for Modals.
    /// </summary>
    public interface IModalInteraction : IDiscordInteraction
    {
        /// <summary>
        ///     Gets the data received with this interaction, contains the button that was clicked.
        /// </summary>
        new IModalInteractionData Data { get; }
    }
}
