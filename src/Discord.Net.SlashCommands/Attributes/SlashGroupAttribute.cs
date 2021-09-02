using System;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Create nested Application Commands.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SlashGroupAttribute : Attribute
    {
        /// <summary>
        /// Name of the group
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Description of the group
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Tag a class or a method as a group
        /// </summary>
        /// <param name="name">Name of the group</param>
        /// <param name="description">Description of the group</param>
        public SlashGroupAttribute (string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
