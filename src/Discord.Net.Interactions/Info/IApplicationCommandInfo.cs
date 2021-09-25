namespace Discord.Interactions
{
    /// <summary>
    ///     Represents a <see cref="InteractionService"/> command that can be registered to Discord
    /// </summary>
    public interface IApplicationCommandInfo
    {
        /// <summary>
        ///     Get the name of this command
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Get the type of this command
        /// </summary>
        ApplicationCommandType CommandType { get; }

        /// <summary>
        ///     Get the DefaultPermission of this command
        /// </summary>
        bool DefaultPermission { get; }
    }
}
