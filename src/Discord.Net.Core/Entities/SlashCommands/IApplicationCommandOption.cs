using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    /// Represents an Application Command Option, this may be a Sub-Command, a Sub-Command group or a command parameter
    /// </summary>
    public interface IApplicationCommandOption
    {
        /// <summary>
        /// Get the type of this option
        /// </summary>
        ApplicationCommandOptionType OptionType { get; }
        /// <summary>
        /// Get the name of this option
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Get the description of this option
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Wheter this option is essential for the execution of an Application Comand
        /// </summary>
        /// <remarks>
        /// <see langword="false"/> by default for types <see cref="ApplicationCommandOptionType.SubCommand"/> and <see cref="ApplicationCommandOptionType.SubCommandGroup"/>
        /// </remarks>
        bool IsRequired { get; }

        /// <summary>
        /// The collection of Dev-defined choices for this option, if this option is a parameter
        /// </summary>
        IEnumerable<KeyValuePair<string, object>> Choices { get; }

        /// <summary>
        /// The collection of Sub-Commands and Sub-Command Groups, null if this option is a command parameter
        /// </summary>
        IEnumerable<IApplicationCommandOption> Options { get; }
    }
}
