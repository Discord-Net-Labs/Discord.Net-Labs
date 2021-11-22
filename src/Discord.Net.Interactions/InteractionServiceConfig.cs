using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents a configuration class for <see cref="InteractionService"/>
    /// </summary>
    public class InteractionServiceConfig
    {
        /// <summary>
        ///     Gets or sets the minimum log level severity that will be sent to the <see cref="InteractionService.Log"/> event.
        /// </summary>
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;

        /// <summary>
        ///     Gets or sets the default <see cref="RunMode" /> commands should have, if one is not specified on the
        ///     Command attribute or builder.
        /// </summary>
        public RunMode DefaultRunMode { get; set; } = RunMode.Async;

        /// <summary>
        ///     Gets or sets whether <see cref="RunMode.Sync"/> commands should push exceptions up to the caller.
        /// </summary>
        public bool ThrowOnError { get; set; } = true;

        /// <summary>
        ///     Delimiters that will be used to seperate group names and the method name when a Message Component Interaction is recieved
        /// </summary>
        public char[] InteractionCustomIdDelimiters { get; set; }

        public char[] AutocompleteNameDelimiters { get; set; }

        /// <summary>
        ///     The string expression that will be treated as a wild card
        /// </summary>
        public string WildCardExpression { get; set; }

        /// <summary>
        ///     Delete Slash Command acknowledgements if no Slash Command handler is found in the <see cref="InteractionService"/>
        /// </summary>
        public bool DeleteUnknownSlashCommandAck { get; set; } = true;

        /// <summary>
        ///     Use compiled lambda expressions to create module instances and execute commands. This method improves performance at the cost of memory
        /// </summary>
        public bool UseCompiledLambda { get; set; } = false;

        /// <summary>
        ///     Allows you to use <see cref="Autocompleter"/>s
        /// </summary>
        /// <remarks>
        ///     Since <see cref="Autocompleter"/>s are prioritized over <see cref="AutocompleteCommandInfo"/>s, if <see cref="Autocompleter"/>s are not used, this should be
        ///     disabled to decrease the lookup time
        /// </remarks>
        public bool EnableAutocompleters { get; set; } = true;

        /// <summary>
        ///     Define a delegate to be used by the <see cref="InteractionService"/> when responding to a Rest based interaction
        /// </summary>
        public Func<string, Task> RestResponseCallback { get; set; } = (str) => Task.CompletedTask;
    }
}
