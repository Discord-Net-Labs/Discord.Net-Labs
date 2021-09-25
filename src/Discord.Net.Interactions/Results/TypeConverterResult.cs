using System;

namespace Discord.Interactions
{
    public struct TypeConverterResult : IResult
    {
        public object Value { get; }

        /// <inheritdoc/>
        public InteractionCommandError? Error { get; }

        /// <inheritdoc/>
        public string ErrorReason { get; }

        /// <inheritdoc/>
        public bool IsSuccess => !Error.HasValue;

        private TypeConverterResult (object value, InteractionCommandError? error, string reason)
        {
            Value = value;
            Error = error;
            ErrorReason = reason;
        }

        public static TypeConverterResult FromSuccess (object value) =>
            new TypeConverterResult(value, null, null);

        public static TypeConverterResult FromError (Exception ex) =>
            new TypeConverterResult(null, InteractionCommandError.Exception, ex.Message);

        public static TypeConverterResult FromError (InteractionCommandError error, string reason) =>
            new TypeConverterResult(null, error, reason);

        public static TypeConverterResult FromError (IResult result) =>
            new TypeConverterResult(null, result.Error, result.ErrorReason);

        public override string ToString ( ) => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
