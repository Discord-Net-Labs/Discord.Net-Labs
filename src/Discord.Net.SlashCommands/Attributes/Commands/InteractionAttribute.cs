using System;

namespace Discord.SlashCommands
{
    /// <summary>
    /// Create a Message Component interaction handler, CustomId represents
    /// the CustomId of the Message Component that will be handled
    /// </summary>
    /// <remarks>
    /// This will be affected by <see cref="SlashGroupAttribute"/>.
    /// CustomID supports a Wild Card pattern where you can use the <see cref="SlashCommandServiceConfig.WildCardExpression"/> to match a set of CustomIDs
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class InteractionAttribute : Attribute
    {
        /// <summary>
        /// String to compare the Message Component CustomIDs with
        /// </summary>
        public string CustomId { get; }

        /// <summary>
        /// If <see langword="true"/> <see cref="SlashGroupAttribute"/>s will be ignored while creating this command and this method will be treated as a top level command
        /// </summary>
        public bool IgnoreGroupNames { get; set; } = false;

        /// <summary>
        /// Create a command for interaction handling
        /// </summary>
        /// <param name="customId">String to compare the Message Component CustomIDs with</param>
        public InteractionAttribute (string customId)
        {
            CustomId = customId;
        }
    }
}
