using System;
using System.Collections.Generic;
using System.Text;

namespace Discord.SlashCommands
{
    /// <summary>
    /// 
    /// </summary>
    public struct TypeReaderResult : IResult
    {
        /// <summary>
        /// 
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Type of the error that caused reading process to fail
        /// </summary>
        public SlashCommandError? Error { get; }

        /// <summary>
        /// The reason why the reading process failed
        /// </summary>
        public string ErrorReason { get; }

        /// <summary>
        /// <see langword="true"/> if the option was read successfully
        /// </summary>
        public bool IsSuccess => !Error.HasValue;

        private TypeReaderResult ( object value, SlashCommandError? error, string reason )
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
