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
        ///     A space seperated collection of names that point to the target parameter
        /// </summary>
        public string Name { get; }

        public RunMode RunMode { get; }

        /// <summary>
        ///     Create a command for Autocomplete interaction handling
        /// </summary>
        /// <param name="name"></param>
        /// /// <param name="runMode">Set the run mode of the command</param>
        public AutocompleteCommandAttribute(string name, RunMode runMode = RunMode.Default)
        {
            Name = name;
            RunMode = runMode;
        }
    }
}
