using System;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Use to create an Application Command. Stacks with <see cref="SlashGroupAttribute"/>
    /// </summary>
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
