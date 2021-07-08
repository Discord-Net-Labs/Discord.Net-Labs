using System;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Attribute for tagging a Message Component interaction handler, CustomId represents
    /// the CustomId of the Message Component that will be handled. This will stack with <see cref="SlashGroupAttribute"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class InteractionAttribute : Attribute
    {
        /// <summary>
        /// Custom ID of the Message Component that raises the event which will be handled
        /// </summary>
        public string CustomId { get; }

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
