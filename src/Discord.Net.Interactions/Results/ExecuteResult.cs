using System;

namespace Discord.Interactions
{
    /// <summary>
    /// Represents the result of a command execution operation
    /// </summary>
    public struct ExecuteResult : IResult
    {
        /// <summary>
        /// Get the exception that caused the execution to fail, if the operation was not successful
        /// </summary>
        public Exception Exception { get; }

        /// <inheritdoc/>
        public InteractionCommandError? Error { get; }

        /// <inheritdoc/>
        public string ErrorReason { get; }

        /// <inheritdoc/>
        public bool IsSuccess => !Error.HasValue;

        private ExecuteResult (Exception exception, InteractionCommandError? commandError, string errorReason)
        {
            Exception = exception;
            Error = commandError;
            ErrorReason = errorReason;
        }

        public static ExecuteResult FromSuccess ( ) =>
            new ExecuteResult(null, null, null);

        public static ExecuteResult FromError (InteractionCommandError commandError, string reason) =>
            new ExecuteResult(null, commandError, reason);

        public static ExecuteResult FromError (Exception exception) =>
            new ExecuteResult(exception, InteractionCommandError.Exception, exception.Message);

        public static ExecuteResult FromError (IResult result) =>
            new ExecuteResult(null, result.Error, result.ErrorReason);

        public override string ToString ( ) => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
