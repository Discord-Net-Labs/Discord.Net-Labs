using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Create an Slash Application Command.
    /// </summary>
    /// <remarks>
    ///     <see cref="GroupAttribute"/> prefix will be used to created nested Slash Application Commands
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SlashCommandAttribute : Attribute
    {
        /// <summary>
        ///     Name of the Slash Command
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Description of the Slash Command
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Type of this Application Command
        /// </summary>
        /// <remarks>
        ///     Will always be <see cref="ApplicationCommandType.Slash"/>
        /// </remarks>
        public ApplicationCommandType CommandType => ApplicationCommandType.Slash;

        /// <summary>
        ///     If <see langword="true"/>, <see cref="GroupAttribute"/>s will be ignored while creating this command and this method will be treated as a top level command
        /// </summary>
        public bool IgnoreGroupNames { get; set; } = false;

        public RunMode RunMode { get; set; } = RunMode.Default;

        /// <summary>
        ///     Register a method as a Slash Command
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
