using System;
using System.Collections.Generic;
using System.Text;

namespace Discord.SlashCommands
{
    internal struct SearchResult<T> : IResult where T: class, IExecutableInfo
    {
        public string Text { get; }
        public T Command { get; }
        public SlashCommandError? Error { get; }

        public string ErrorReason { get; }

        public bool IsSuccess => !Error.HasValue;

        private SearchResult(string text, T commandInfo, SlashCommandError? error, string reason)
        {
            Text = text;
            Error = error;
            Command = commandInfo;
            ErrorReason = reason;
        }

        public static SearchResult<T> FromSuccess (string text, T commandInfo) =>
            new SearchResult<T>(text, commandInfo, null, null);

        public static SearchResult<T> FromError (string text, SlashCommandError error, string reason) =>
            new SearchResult<T>(text, null, error, reason);
        public static SearchResult<T> FromError (Exception ex) =>
            new SearchResult<T>(null, null, SlashCommandError.Exception, ex.Message);
        public static SearchResult<T> FromError (IResult result) =>
            new SearchResult<T>(null, null, result.Error, result.ErrorReason);

        public override string ToString ( ) => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
