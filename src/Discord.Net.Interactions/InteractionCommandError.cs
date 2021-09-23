namespace Discord.Interactions
{
    /// <summary>
    /// Error types for the <see cref="InteractionService"/> executables
    /// </summary>
    public enum InteractionCommandError
    {
        /// <summary>
        /// Thrown when no registered command is found for a given input
        /// </summary>
        UnknownCommand,

        /// <summary>
        /// Thrown when a Discord Application Command fails to be parsed from an <see cref="ICommandInfo"/>
        /// </summary>
        ParseFailed,

        /// <summary>
        /// Thrown when the provided command arguments does not match the method arguments
        /// </summary>
        BadArgs,

        /// <summary>
        /// Thrown whenever an exception occurs during the command execution process
        /// </summary>
        Exception,

        /// <summary>
        /// Thrown when the command is not successfully executed on runtime.
        /// </summary>
        Unsuccessful,
        UnmetPrecondition
    }
}
