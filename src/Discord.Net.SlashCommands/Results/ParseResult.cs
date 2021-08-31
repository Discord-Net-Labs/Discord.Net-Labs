using System;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Represents the result of a Application Command Parsing process from an <see cref="IExecutableInfo"/>
    /// </summary>
    public struct ParseResult : IResult
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

        private ParseResult (IApplicationCommand command, SlashCommandError? error, string reason)
        {
            Command = command;
            Error = error;
            ErrorReason = reason;
        }

        public static ParseResult FromSuccess (IApplicationCommand command) =>
            new ParseResult(command, null, null);

        public static ParseResult FromError (Exception ex) =>
            new ParseResult(null, SlashCommandError.Exception, ex.Message);

        public static ParseResult FromError (IResult result) =>
            new ParseResult(null, result.Error, result.ErrorReason);

        public static ParseResult FromError (SlashCommandError error, string reason) =>
            new ParseResult(null, error, reason);

        public override string ToString ( ) => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
