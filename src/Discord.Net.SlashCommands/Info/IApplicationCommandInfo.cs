namespace Discord.SlashCommands
{
    /// <summary>
    /// Represents a <see cref="SlashCommandService"/> command that can be registered to Discord
    /// </summary>
    public interface IApplicationCommandInfo
    {
        /// <summary>
        /// Get the name of this command
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get the type of this command
        /// </summary>
        ApplicationCommandType CommandType { get; }
    }
}
