namespace Discord
{
    /// <summary>
    /// Types of Interaction Responses
    /// </summary>
    public enum InteractionCallbackType
    {
        /// <summary>
        /// Response for the type of <see cref="InteractionType.Ping"/>
        /// </summary>
        Pong = 1,
        /// <summary>
        /// Response for the type of <see cref="InteractionType.ApplicationCommand"/>
        /// </summary>
        ChannelMessageWithSource = 4,
        /// <summary>
        /// Acknowledgement for the type of <see cref="InteractionType.ApplicationCommand"/>
        /// </summary>
        DeferredChannelMessageWithSource = 5,
        /// <summary>
        /// Acknowledgement for the type of <see cref="InteractionType.MessageComponent"/>
        /// </summary>
        DeferredUpdateMessage = 6,
        /// <summary>
        /// Response for the type of <see cref="InteractionType.MessageComponent"/>
        /// </summary>
        UpdateMessage = 7
    }
}
