using System;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Use to change the default name and description of an Application Command parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class SummaryAttribute : Attribute
    {
        /// <summary>
        /// Custom name of the parameter
        /// </summary>
        public string Name { get; set; } = null;
        /// <summary>
        /// Custom description of the parameter
        /// </summary>
        public string Description { get; } = null;

        /// <summary>
        /// Modify the default name and description values of a Slash Command parameter
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="description">Description of the parameter</param>
        public SummaryAttribute (string name = null, string description = null)
        {
            Name = name;
            Description = description;
        }
    }
}
