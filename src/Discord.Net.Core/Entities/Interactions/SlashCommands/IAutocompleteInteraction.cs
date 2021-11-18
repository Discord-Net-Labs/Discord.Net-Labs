namespace Discord
{
    /// <summary>
    ///     Represents a <see cref="InteractionType.ApplicationCommandAutocomplete"/> received over the gateway.
    /// </summary>
    public interface IAutocompleteInteraction : IDiscordInteraction
    {
        /// <summary>
        ///     The autocomplete data of this interaction.
        /// </summary>
        new IAutocompleteInteractionData Data { get; }
    }
}
