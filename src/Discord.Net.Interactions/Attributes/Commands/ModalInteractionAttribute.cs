using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Create a Modal interaction handler, CustomId represents
    ///     the CustomId of the Modal that will be handled.
    /// </summary>
    /// <remarks>
    ///     <see cref="GroupAttribute"/>s will add prefixes to this command if <see cref="IgnoreGroupNames"/> is set to <see langword="false"/>
    ///     CustomID supports a Wild Card pattern where you can use the <see cref="InteractionServiceConfig.WildCardExpression"/> to match a set of CustomIDs.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ModalInteractionAttribute : Attribute
    {
        /// <summary>
        ///     Gets the string to compare the Modal CustomIDs with.
        /// </summary>
        public string CustomId { get; }

        /// <summary>
        ///     Gets the title of the modal.
        /// </summary>
        public string Title { get; }
        
        /// <summary>
        ///     Gets <see langword="true"/> if <see cref="GroupAttribute"/>s will be ignored while creating this command and this method will be treated as a top level command.
        /// </summary>
        public bool IgnoreGroupNames { get; }

        /// <summary>
        ///     Gets the run mode this command gets executed with.
        /// </summary>
        public RunMode RunMode { get; }

        /// <summary>
        ///     Create a command for modal interaction handling.
        /// </summary>
        /// <param name="customId">String to compare the modal CustomIDs with.</param>
        /// <param name="ignoreGroupNames">If <see langword="true"/> <see cref="GroupAttribute"/>s will be ignored while creating this command and this method will be treated as a top level command.</param>
        /// <param name="runMode">Set the run mode of the command.</param>
        public ModalInteractionAttribute(string customId, string title, bool ignoreGroupNames = false, RunMode runMode = RunMode.Default)
        {
            CustomId = customId;
            Title = title;
            IgnoreGroupNames = ignoreGroupNames;
            RunMode = runMode;
        }
    }
}
