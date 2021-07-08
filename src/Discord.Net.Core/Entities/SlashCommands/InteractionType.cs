namespace Discord
{
    /// <summary>
    /// Types of interaction that can be recieved from Discord
    /// </summary>
    public enum InteractionType
    {
        Ping = 1,
        /// <summary>
        /// Recieved when a Slash Command is used
        /// </summary>
        ApplicationCommand = 2,
        /// <summary>
        /// Recieved when a user uses a <see cref="MessageComponent"/>
        /// </summary>
        MessageComponent = 3
    }
}
