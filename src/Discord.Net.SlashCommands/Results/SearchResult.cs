using System;
using System.Collections.Generic;
using System.Text;

namespace Discord.SlashCommands
{
    internal struct SearchResult<T> : IResult where T: class, IExecutableInfo
    {
        public string Text { get; }
        public T Command { get; }
        public string[] WilCardMatch { get; }
        public SlashCommandError? Error { get; }

        public string ErrorReason { get; }

        public bool IsSuccess => !Error.HasValue;

        private SearchResult(string text, T commandInfo, string[] wildCardMatch, SlashCommandError? error, string reason)
        {
            Text = text;
            Error = error;
            WilCardMatch = wildCardMatch;
            Command = commandInfo;
            ErrorReason = reason;
        }

        public static SearchResult<T> FromSuccess (string text, T commandInfo, string[] wildCardMatch = null) =>
            new SearchResult<T>(text, commandInfo, wildCardMatch, null, null);

        public static SearchResult<T> FromError (string text, SlashCommandError error, string reason) =>
            new SearchResult<T>(text, null, null, error, reason);
        public static SearchResult<T> FromError (Exception ex) =>
            new SearchResult<T>(null, null, null, SlashCommandError.Exception, ex.Message);
        public static SearchResult<T> FromError (IResult result) =>
            new SearchResult<T>(null, null, null, result.Error, result.ErrorReason);

        public override string ToString ( ) => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
