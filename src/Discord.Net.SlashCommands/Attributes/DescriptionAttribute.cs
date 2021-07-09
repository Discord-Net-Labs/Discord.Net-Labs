using System;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Use to change the default name and description of an Application Command element
    /// </summary>
    public class DescriptionAttribute : Attribute
    {
        /// <summary>
        /// Custom name of the element
        /// </summary>
        public string Name { get; set; } = null;
        /// <summary>
        /// Custom description of the element
        /// </summary>
        public string Description { get; } = null;

        /// <summary>
        /// Modify the default name and description values of a Slash Command element
        /// </summary>
        /// <param name="name">Name of the element</param>
        /// <param name="description">Description of the element</param>
        public DescriptionAttribute (string name, string description = null)
        {
            Name = name;
            Description = description;
        }
    }
}
