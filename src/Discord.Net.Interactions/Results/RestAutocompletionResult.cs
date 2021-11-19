using System;
using System.Collections.Generic;

namespace Discord.Interactions
{
    /// <summary>
    ///     Contains the information of a Rest based Autocomplete Interaction result.
    /// </summary>
    public class RestAutocompletionResult : AutocompletionResult
    {
        /// <summary>
        ///     A string that contains json to write back to the incoming http request.
        /// </summary>
        public string SerializedPayload { get; }

        private RestAutocompletionResult(IEnumerable<AutocompleteResult> suggestions, string serializedPayload, InteractionCommandError? error, string reason) : base(suggestions, error, reason)
        {
            SerializedPayload = serializedPayload;
        }

        /// <summary>
        ///     Initializes a new <see cref="AutocompletionResult" /> with no error.
        /// </summary>
        /// <param name="suggestions">Autocomplete suggestions to be displayed to the user</param>
        /// <param name="serializedPayload">A string that contains json to write back to the incoming http request.</param>
        /// <returns>
        ///     A <see cref="AutocompletionResult" /> that does not contain any errors.
        /// </returns>
        internal static RestAutocompletionResult FromSuccess(IEnumerable<AutocompleteResult> suggestions, string serializedPayload) =>
            new RestAutocompletionResult(suggestions, serializedPayload, null, null);

        /// <summary>
        ///     Initializes a new <see cref="AutocompletionResult" /> with a specified result; this may or may not be an
        ///     successful execution depending on the <see cref="IResult.Error" /> and
        ///     <see cref="IResult.ErrorReason" /> specified.
        /// </summary>
        /// <param name="result">The result to inherit from.</param>
        /// <returns>
        ///     A <see cref="AutocompletionResult"/> that inherits the <see cref="IResult"/> error type and reason.
        /// </returns>
        internal new static RestAutocompletionResult FromError(IResult result) =>
            new RestAutocompletionResult(null, null, result.Error, result.ErrorReason);

        /// <summary>
        ///     Initializes a new <see cref="AutocompletionResult" /> with a specified exception, indicating an unsuccessful
        ///     execution.
        /// </summary>
        /// <param name="exception">The exception that caused the autocomplete process to fail.</param>
        /// <returns>
        ///     A <see cref="AutocompletionResult" /> that contains the exception that caused the unsuccessful execution, along
        ///     with a <see cref="InteractionCommandError" /> of type <see cref="Exception"/> as well as the exception message as the
        ///     reason.
        /// </returns>
        internal new static RestAutocompletionResult FromError(Exception exception) =>
            new RestAutocompletionResult(null, null, InteractionCommandError.Exception, exception.Message);

        /// <summary>
        ///     Initializes a new <see cref="AutocompletionResult" /> with a specified <see cref="InteractionCommandError" /> and its
        ///     reason, indicating an unsuccessful execution.
        /// </summary>
        /// <param name="error">The type of error.</param>
        /// <param name="reason">The reason behind the error.</param>
        /// <returns>
        ///     A <see cref="AutocompletionResult" /> that contains a <see cref="InteractionCommandError" /> and reason.
        /// </returns>
        internal new static RestAutocompletionResult FromError(InteractionCommandError error, string reason) =>
            new RestAutocompletionResult(null, null, error, reason);
    }
}
