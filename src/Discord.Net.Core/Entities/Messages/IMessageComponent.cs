namespace Discord
{
    /// <summary>
    /// Represents a Message Component
    /// </summary>
    public interface IMessageComponent
    {
        /// <summary>
        /// Type of the component
        /// </summary>
        MessageComponentType ComponentType { get; }
    }
}
