using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     The base command model that belongs to an application. see <see href="https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-structure"/>
    /// </summary>
    public interface IApplicationCommand : ISnowflakeEntity, IDeletable
    {
        /// <summary>
        ///     The type of command.
        /// </summary>
        ApplicationCommandType Type { get; } 

        /// <summary>
        ///     Gets the unique id of the parent application.
        /// </summary>
        ulong ApplicationId { get; }

        /// <summary>
        ///     The 1-32 character name of the command.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The 1-100 character description of the command.
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     Whether the command is enabled by default when the app is added to a guild.
        /// </summary>
        bool DefaultPermission { get; }

        /// <summary>
        ///     The parameters for the command, max 25.
        /// </summary>
        /// <remarks>
        ///     Only valid for commands with a type of <see cref="ApplicationCommandType.ChatInput"/>.
        /// </remarks>
        IReadOnlyCollection<IApplicationCommandOption> Options { get; }
    }
}
