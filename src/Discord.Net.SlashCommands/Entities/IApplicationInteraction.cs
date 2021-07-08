namespace Discord.SlashCommands
{
    /// <summary>
    /// Represents an Interaction event
    /// </summary>
    public interface IApplicationInteraction : ISnowflakeEntity, IDeletable
    {
        /// <summary>
        /// ID of the application the interaction was sent to
        /// </summary>
        ulong ApplicationId { get; }

        /// <summary>
        /// Type of the interaction
        /// </summary>
        InteractionType Type { get; }

        /// <summary>
        /// User who created the interaction
        /// </summary>
        IUser User { get; set; }
    }
}
