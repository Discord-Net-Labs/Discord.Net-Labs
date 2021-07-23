using System;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Use to create nested Application Commands.
    /// </summary>
    /// <remarks>
    /// Can be either used to mark a class to tag all of the declared methods or to mark commands individually
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SlashGroupAttribute : Attribute
    {
        /// <summary>
        /// Name of the group
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Desription of the group
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
