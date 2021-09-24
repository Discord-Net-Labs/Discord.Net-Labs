using System;

namespace Discord.Interactions
{
    /// <summary>
    /// Create a Message Component interaction handler, CustomId represents
    /// the CustomId of the Message Component that will be handled
    /// </summary>
    /// <remarks>
    /// This will be affected by <see cref="GroupAttribute"/>.
    /// CustomID supports a Wild Card pattern where you can use the <see cref="InteractionServiceConfig.WildCardExpression"/> to match a set of CustomIDs
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ComponentInteractionAttribute : Attribute
    {
        /// <summary>
        /// String to compare the Message Component CustomIDs with
        /// </summary>
        public string CustomId { get; }

        /// <summary>
        /// If <see langword="true"/> <see cref="GroupAttribute"/>s will be ignored while creating this command and this method will be treated as a top level command
        /// </summary>
        public bool IgnoreGroupNames { get; set; } = false;

        public RunMode RunMode { get; set; } = RunMode.Default;

        /// <summary>
        /// Create a command for interaction handling
        /// </summary>
        /// <param name="customId">String to compare the Message Component CustomIDs with</param>
        public ComponentInteractionAttribute (string customId)
        {
            CustomId = customId;
        }
    }
}
