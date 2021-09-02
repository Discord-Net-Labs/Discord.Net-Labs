using System;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Represents the result of a Application Command fetch process from an <see cref="IExecutableInfo"/>
    /// </summary>
    public struct FetchResult : IResult
    {
        /// <summary>
        /// Get the parsed application command if the operation was successful
        /// </summary>
        public IApplicationCommand Command { get; }

        /// <inheritdoc/>
        public SlashCommandError? Error { get; }

        /// <inheritdoc/>
        public string ErrorReason { get; }

        /// <inheritdoc/>
        public bool IsSuccess => !Error.HasValue;

        private FetchResult (IApplicationCommand command, SlashCommandError? error, string reason)
        {
            Command = command;
            Error = error;
            ErrorReason = reason;
        }

        public static FetchResult FromSuccess (IApplicationCommand command) =>
            new FetchResult(command, null, null);

        public static FetchResult FromError (Exception ex) =>
            new FetchResult(null, SlashCommandError.Exception, ex.Message);

        public static FetchResult FromError (IResult result) =>
            new FetchResult(null, result.Error, result.ErrorReason);

        public static FetchResult FromError (SlashCommandError error, string reason) =>
            new FetchResult(null, error, reason);

        public override string ToString ( ) => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
