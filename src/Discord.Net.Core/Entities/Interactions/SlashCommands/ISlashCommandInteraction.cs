namespace Discord
{
    /// <summary>
    ///     Represents a slash command interaction received over the gateway.
    /// </summary>
    public interface ISlashCommandInteraction : IDiscordInteraction
    {
        /// <summary>
        ///     The data associated with this interaction.
        /// </summary>
        new IApplicationCommandInteractionData Data { get; }
    }
}
