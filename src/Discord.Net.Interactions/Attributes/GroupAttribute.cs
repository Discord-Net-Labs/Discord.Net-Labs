using System;

namespace Discord.Interactions
{
    /// <summary>
    /// Create nested Application Commands by marking the module as a command group
    /// </summary>
    /// <remarks>
    /// Groups dont apply to <see cref="ContextCommandAttribute"/> commands. 
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GroupAttribute : Attribute
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
        /// Initialize a command group with the provided name and description
        /// </summary>
        /// <param name="name">Name of the group</param>
        /// <param name="description">Description of the group</param>
        public GroupAttribute (string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
