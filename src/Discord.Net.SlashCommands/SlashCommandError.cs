namespace Discord.SlashCommands
{
    /// <summary>
    /// Error types for the <see cref="SlashCommandService"/> executables
    /// </summary>
    public enum SlashCommandError
    {
        /// <summary>
        /// Thrown when no registered command is found for a given input
        /// </summary>
        UnknownCommand = 1,

        /// <summary>
        /// Thrown when a Discord Application Command fails to be parsed from an <see cref="IExecutableInfo"/>
        /// </summary>
        ParseFailed = 2,

        /// <summary>
        /// Thrown when the provided command arguments does not match the method arguments
        /// </summary>
        BadArgs = 3,

        /// <summary>
        /// Thrown whenever an exception occurs during the command execution process
        /// </summary>
        Exception = 4,

        /// <summary>
        /// Thrown when the command is not successfully executed on runtime.
        /// </summary>
        Unsuccessful = 5
    }
}
