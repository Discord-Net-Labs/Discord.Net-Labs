using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents a option group for a command, see <see href="https://discord.com/developers/docs/interactions/slash-commands#interaction-applicationcommandinteractiondataoption"/>.
    /// </summary>
    public interface IApplicationCommandInteractionDataOption
    {
        /// <summary>
        ///     The name of the parameter.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The value of the pair.
        ///     <note>
        ///         This objects type can be any one of the option types in <see cref="ApplicationCommandOptionType"/>
        ///     </note>
        /// </summary>
        object Value { get; }

        /// <summary>
        ///     The type of this data's option.
        /// </summary>
        ApplicationCommandOptionType Type { get; }

        /// <summary>
        ///     Present if this option is a group or subcommand.
        /// </summary>
        IReadOnlyCollection<IApplicationCommandInteractionDataOption> Options { get; }
    }
}
