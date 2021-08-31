namespace Discord.SlashCommands
{
    /// <summary>
    /// Represents an operation result
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Get the error type if this result was not successful
        /// </summary>
        SlashCommandError? Error { get; }
        /// <summary>
        /// Get the error reason that caused the error
        /// </summary>
        string ErrorReason { get; }
        /// <summary>
        /// Wheter the operation that produced this result was successful
        /// </summary>
        bool IsSuccess { get; }
    }
}
