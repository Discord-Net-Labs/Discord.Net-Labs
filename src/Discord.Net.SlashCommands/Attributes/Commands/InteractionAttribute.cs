using System;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Attribute for tagging a Message Component interaction handler, CustomId represents
    /// the CustomId of the Message Component that will be handled
    /// </summary>
    /// <remarks>
    /// This will be affected by <see cref="SlashGroupAttribute"/>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class InteractionAttribute : Attribute
    {
        /// <summary>
        /// Custom ID of the Message Component that raises the event which will be handled
        /// </summary>
        public string CustomId { get; }

        /// <summary>
        /// Wheter the <see cref="SlashGroupAttribute"/>s should be ignored while creating this command
        /// </summary>
        public bool IgnoreGroupNames { get; set; } = false;

        /// <summary>
        /// Tag a method for interaction handling
        /// </summary>
        /// <param name="customId">Custom ID of the Message Component that raises the event which will be handled</param>
        public InteractionAttribute (string customId)
        {
            CustomId = customId;
        }
    }
}
