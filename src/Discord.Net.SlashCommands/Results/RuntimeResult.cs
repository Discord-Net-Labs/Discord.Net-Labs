namespace Discord.SlashCommands
{
    /// <summary>
    /// Represents the result returned by a command method
    /// </summary>
    public abstract class RuntimeResult : IResult
    {
        /// <inheritdoc/>
        public SlashCommandError? Error { get; }
        /// <inheritdoc/>
        public string ErrorReason { get; }
        /// <inheritdoc/>
        public bool IsSuccess => !Error.HasValue;

        protected RuntimeResult (SlashCommandError? error, string reason)
        {
            Error = error;
            ErrorReason = reason;
        }

        public override string ToString ( ) => ErrorReason ?? ( IsSuccess ? "Successful" : "Unsuccessful" );
    }
}
