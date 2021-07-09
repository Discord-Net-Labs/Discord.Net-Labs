namespace Discord
{
    /// <summary>
    /// Represents a UI element that can be sent with a message
    /// </summary>
    public abstract class MessageComponent : IMessageComponent
    {
        /// <summary>
        /// Type of the Component
        /// </summary>
        public MessageComponentType ComponentType { get; }

        internal MessageComponent (MessageComponentType type)
        {
            ComponentType = type;
        }
    }
}
