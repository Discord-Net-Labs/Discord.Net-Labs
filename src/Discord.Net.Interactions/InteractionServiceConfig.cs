namespace Discord.Interactions
{
    /// <summary>
    /// Holds the general information to be used while initializing <see cref="InteractionCommandContext"/>
    /// </summary>
    public class InteractionServiceConfig
    {
        /// <summary>
        /// Log severity for the logger
        /// </summary>
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;
        /// <summary>
        /// Run mode that will be used when executing the commands
        /// </summary>
        public RunMode RunMode { get; set; } = RunMode.Async;
        public bool ThrowOnError { get; set; } = true;

        /// <summary>
        /// Delimiters that will be used to seperate group names and the method name when a Message Component Interaction is recieved
        /// </summary>
        public char[] InteractionCustomIdDelimiters { get; set; }

        /// <summary>
        /// The string expression that will be treated as a wild card
        /// </summary>
        public string WildCardExpression { get; set; }

        /// <summary>
        /// Delete Slash Command acknowledgements if no Slash Command handler is found in the <see cref="InteractionService"/>
        /// </summary>
        public bool DeleteUnknownSlashCommandAck { get; set; } = true;

        public bool UseCompiledLambda { get; set; } = false;
    }
}
