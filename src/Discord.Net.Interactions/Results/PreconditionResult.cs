using System;

namespace Discord.Interactions
{
    /// <summary>
    /// The result type returned by a command precondition
    /// </summary>
    public class PreconditionResult : IResult
    {
        /// <inheritdoc/>
        public InteractionCommandError? Error { get; }

        /// <inheritdoc/>
        public string ErrorReason { get; }

        /// <inheritdoc/>
        public bool IsSuccess => Error == null;

        protected PreconditionResult (InteractionCommandError? error, string reason)
        {
            Error = error;
            ErrorReason = reason;
        }

        public static PreconditionResult FromSuccess ( ) =>
            new PreconditionResult(null, null);

        public static PreconditionResult FromError (Exception exception) =>
            new PreconditionResult(InteractionCommandError.Exception, exception.Message);

        public static PreconditionResult FromError (IResult result) =>
            new PreconditionResult(result.Error, result.ErrorReason);

        public static PreconditionResult FromError (string reason) =>
            new PreconditionResult(InteractionCommandError.UnmetPrecondition, reason);
    }
}
