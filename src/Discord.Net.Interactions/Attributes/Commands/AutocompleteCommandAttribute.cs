using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Create an Autocomplete Command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AutocompleteCommandAttribute : Attribute
    {
        /// <summary>
        ///     Name of the target parameter
        /// </summary>
        public string ParameterName { get; }

        /// <summary>
        ///     Name of the target command
        /// </summary>
        public string CommandName { get; }

        public RunMode RunMode { get; }

        /// <summary>
        ///     Create a command for Autocomplete interaction handling
        /// </summary>
        /// <param name="parameterName">Name of the target parameter</param>
        /// <param name="commandName">Name of the target command</param>
        /// <param name="runMode">Set the run mode of the command</param>
        public AutocompleteCommandAttribute(string parameterName, string commandName, RunMode runMode = RunMode.Default)
        {
            ParameterName = parameterName;
            CommandName = commandName;
            RunMode = runMode;
        }
    }
}
