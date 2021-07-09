namespace Discord
{
    /// <summary>
    /// Represents the context of an Interaction
    /// </summary>
    public interface ISlashCommandContext
    {
        /// <summary>
        /// Client that will be used to handle this interaction
        /// </summary>
        IDiscordClient Client { get; }
        /// <summary>
        /// Guild the interaction originated from
        /// </summary>
        /// <remarks>
        /// Will be <see langword="null"/> if the interaction originated from a DM channel
        /// </remarks>
        IGuild Guild { get; }
        /// <summary>
        /// Channel the interaction originated from
        /// </summary>
        IMessageChannel Channel { get; }
        /// <summary>
        /// User who invoked the interaction event
        /// </summary>
        IUser User { get; }
        /// <summary>
        /// The underlying interaction
        /// </summary>
        IDiscordInteraction Interaction { get; }
    }
}
