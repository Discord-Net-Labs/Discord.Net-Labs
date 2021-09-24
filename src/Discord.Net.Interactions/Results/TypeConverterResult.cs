using System;

namespace Discord.Interactions
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
        public InteractionCommandError? Error { get; }

        /// <summary>
        /// The reason of the error
        /// </summary>
        public string ErrorReason { get; }

        /// <summary>
        /// <see langword="true"/> if the option was read successfully
        /// </summary>
        public bool IsSuccess => !Error.HasValue;

        private TypeConverterResult (object value, InteractionCommandError? error, string reason)
        {
            Value = value;
            Error = error;
            ErrorReason = reason;
        }

        /// <summary>
        /// Create a <see cref="IResult"/> indicating the read process was successful
        /// </summary>
        /// <param name="value">The resulting object</param>
        /// <returns>The result instance</returns>
        public static TypeConverterResult FromSuccess (object value) =>
            new TypeConverterResult(value, null, null);

        /// <summary>
        /// Create a <see cref="IResult"/> indicating the read process failed due to an error
        /// </summary>
        /// <param name="ex">The exception which caused the error</param>
        /// <returns>The result instance</returns>
        public static TypeConverterResult FromError (Exception ex) =>
            new TypeConverterResult(null, InteractionCommandError.Exception, ex.Message);

        /// <summary>
        /// Create a <see cref="IResult"/> indicating the read process failed due to an error
        /// </summary>
        /// <param name="error">Type of the error</param>
        /// <param name="reason">Error reason</param>
        /// <returns>The result instance</returns>
        public static TypeConverterResult FromError (InteractionCommandError error, string reason) =>
            new TypeConverterResult(null, error, reason);

        public static TypeConverterResult FromError (IResult result) =>
            new TypeConverterResult(null, result.Error, result.ErrorReason);

        public override string ToString ( ) => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
