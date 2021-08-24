using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     The base command model that belongs to an application. 
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
        ///     The type of the command
        /// </summary>
        ApplicationCommandType Type { get; }

        /// <summary>
        ///     The name of the command.
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

        /// <summary>
        ///     Modifies the current application command.
        /// </summary>
        /// <param name="func">The new properties to use when modifying the command.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        Task ModifyAsync(Action<ApplicationCommandProperties> func, RequestOptions options = null);
    }
}
