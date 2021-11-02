using System;

namespace Discord.Interactions
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AutocompleteCommandAttribute : Attribute
    {
        public string Name { get; }

        public RunMode RunMode { get; }

        /// <summary>
        ///     Create a command for component interaction handling
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
