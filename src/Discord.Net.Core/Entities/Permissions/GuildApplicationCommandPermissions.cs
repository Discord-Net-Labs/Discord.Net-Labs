using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Returned when fetching the permissions for a command in a guild.
    /// </summary>
    public class GuildApplicationCommandPermission : IApplicationCommandPermission
    {
        /// <summary>
        ///     The id of the command.
        /// </summary>
        public IApplicationCommand Command { get; }

        /// <summary>
        ///     The id of the guild.
        /// </summary>
        public ulong GuildId { get; }

        /// <summary>
        ///     The permissions for the command in the guild.
        /// </summary>
        public IReadOnlyCollection<ApplicationCommandPermission> Permissions { get; }

        internal GuildApplicationCommandPermission(IApplicationCommand command, ulong guildId, ApplicationCommandPermission[] permissions)
        {
            Command = command;
            GuildId = guildId;
            Permissions = permissions;
        }
    }
}
