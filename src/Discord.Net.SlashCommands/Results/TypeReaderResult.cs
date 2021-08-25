using System;
using System.Collections.Generic;
using System.Text;

namespace Discord.SlashCommands
{
    public struct TypeReaderResult : IResult
    {
        public object Value { get; }

        public SlashCommandError? Error { get; }

        public string ErrorReason { get; }

        public bool IsSuccess => !Error.HasValue;

        public TypeReaderResult ( object value, SlashCommandError? error, string reason )
        {
            Value = value;
            Error = error;
            ErrorReason = reason;
        }

        public static TypeReaderResult FromSuccess (object value) =>
            new TypeReaderResult(value, null, null);

        public static TypeReaderResult FromError (Exception ex) =>
            new TypeReaderResult(null, SlashCommandError.Exception, ex.Message);

        public static TypeReaderResult FromError (SlashCommandError error, string reason) =>
            new TypeReaderResult(null, error, reason);

        public static TypeReaderResult FromError (IResult result) =>
            new TypeReaderResult(null, result.Error, result.ErrorReason);

        public override string ToString ( ) => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
