namespace Discord.Interactions
{
    /// <summary>
    ///     Represents a configuration class for <see cref="InteractionCommandContext"/>
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
    }
}
