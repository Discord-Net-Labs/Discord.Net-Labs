using System;

namespace Discord.SlashCommands
{
    /// <summary>
    /// 
    /// </summary>
    public struct TypeConverterResult : IResult
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
        /// The reason of the error
        /// </summary>
        public string ErrorReason { get; }

        /// <summary>
        /// <see langword="true"/> if the option was read successfully
        /// </summary>
        public bool IsSuccess => !Error.HasValue;

        private TypeConverterResult (object value, SlashCommandError? error, string reason)
        {
            Value = value;
            Error = error;
            ErrorReason = reason;
        }

        public static TypeConverterResult FromSuccess (object value) =>
            new TypeConverterResult(value, null, null);

        public static TypeConverterResult FromError (Exception ex) =>
            new TypeConverterResult(null, SlashCommandError.Exception, ex.Message);

        public static TypeConverterResult FromError (SlashCommandError error, string reason) =>
            new TypeConverterResult(null, error, reason);

        public static TypeConverterResult FromError (IResult result) =>
            new TypeConverterResult(null, result.Error, result.ErrorReason);

        public override string ToString ( ) => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
