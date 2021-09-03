namespace Discord.SlashCommands
{
    /// <summary>
    /// Represents an operation result
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Type of the error that caused the process to fail
        /// </summary>
        SlashCommandError? Error { get; }

        /// <summary>
        /// The reason of the error
        /// </summary>
        string ErrorReason { get; }

        /// <summary>
        /// <see langword="true"/> if the operation was successful
        /// </summary>
        bool IsSuccess { get; }
    }
}
