using System;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Use to create an Application Command.
    /// </summary>
    /// /// <remarks>
    /// Can be used alongside with <see cref="SlashGroupAttribute"/> to create nested Discord commands
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SlashCommandAttribute : Attribute
    {
        /// <summary>
        /// Name of the Slash Command
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Description of the Slash Command
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Get the command type of this Slash Command
        /// </summary>
        /// <remarks>
        /// Will always be <see cref="ApplicationCommandType.Slash"/>
        /// </remarks>
        public ApplicationCommandType CommandType => ApplicationCommandType.Slash;

        /// <summary>
        /// If <see langword="true"/>, <see cref="SlashGroupAttribute"/>s will be ignored while creating this command and this method will be treated as a top level command
        /// </summary>
        public bool IgnoreGroupNames { get; set; } = false;

        public RunMode RunMode { get; set; } = RunMode.Default;

        /// <summary>
        /// Register a method as a Slash Command
        /// </summary>
        /// <param name="name">Name of the command</param>
        /// <param name="description">Description of the command</param>
        public SlashCommandAttribute (string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
