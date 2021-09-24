namespace Discord.Interactions
{
    /// <summary>
    /// Represents the result returned by a command method
    /// </summary>
    public abstract class RuntimeResult : IResult
    {
        /// <inheritdoc/>
        public InteractionCommandError? Error { get; }

        /// <inheritdoc/>
        public string ErrorReason { get; }

        /// <inheritdoc/>
        public bool IsSuccess => !Error.HasValue;

        protected RuntimeResult (InteractionCommandError? error, string reason)
        {
            Error = error;
            ErrorReason = reason;
        }

        public override string ToString ( ) => ErrorReason ?? ( IsSuccess ? "Successful" : "Unsuccessful" );
    }
}
