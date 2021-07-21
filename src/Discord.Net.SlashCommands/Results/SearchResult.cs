using System;
using System.Collections.Generic;
using System.Text;

namespace Discord.SlashCommands
{
    public struct SearchResult : IResult
    {
        public string Text { get; }
        public SlashCommandError? Error { get; }

        public string ErrorReason { get; }

        public bool IsSuccess => !Error.HasValue;

        private SearchResult(string text, SlashCommandError? error, string reason)
        {
            Text = text;
            Error = error;
            ErrorReason = reason;
        }

        public static SearchResult FromSuccess (string text) =>
            new SearchResult(text, null, null);

        public static SearchResult FromError (string text, SlashCommandError error, string reason) =>
            new SearchResult(text, error, reason);
        public static SearchResult FromError (Exception ex) =>
            new SearchResult(null, SlashCommandError.Exception, ex.Message);
        public static SearchResult FromError (IResult result) =>
            new SearchResult(null, result.Error, result.ErrorReason);

        public override string ToString ( ) => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
