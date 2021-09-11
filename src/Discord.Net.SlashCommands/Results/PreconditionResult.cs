using System;

namespace Discord.SlashCommands
{
    /// <summary>
    /// The result type returned by a command precondition
    /// </summary>
    public class PreconditionResult : IResult
    {
        /// <inheritdoc/>
        public SlashCommandError? Error { get; }

        /// <inheritdoc/>
        public string ErrorReason { get; }

        /// <inheritdoc/>
        public bool IsSuccess => Error == null;

        protected PreconditionResult (SlashCommandError? error, string reason)
        {
            Error = error;
            ErrorReason = reason;
        }

        public static PreconditionResult FromSuccess ( ) =>
            new PreconditionResult(null, null);

        public static PreconditionResult FromError (Exception exception) =>
            new PreconditionResult(SlashCommandError.Exception, exception.Message);

        public static PreconditionResult FromError (IResult result) =>
            new PreconditionResult(result.Error, result.ErrorReason);

        public static PreconditionResult FromError (string reason) =>
            new PreconditionResult(SlashCommandError.UnmetPrecondition, reason);
    }
}
