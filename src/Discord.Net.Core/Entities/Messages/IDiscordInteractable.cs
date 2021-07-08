namespace Discord
{
    /// <summary>
    /// Represents an interacable Message Component
    /// </summary>
    public interface IDiscordInteractable
    {
        /// <summary>
        /// Dev-defined unique identifier of the component
        /// </summary>
        string CustomId { get; }
    }
}
