using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.ApplicationCommand;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a Rest-based implementation of the <see cref="IApplicationCommand"/>.
    /// </summary>
    public abstract class RestApplicationCommand : RestEntity<ulong>, IApplicationCommand
    {
        /// <inheritdoc/>
        public ulong ApplicationId { get; private set; }

        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public string Description { get; private set; }

        /// <inheritdoc/>
        public bool DefaultPermission { get; private set; }

        /// <summary>
        ///     The options of this command.
        /// </summary>
        public IReadOnlyCollection<ApplicationCommandOption> Options { get; private set; }

        /// <summary>
        ///     The type of this rest application command.
        /// </summary>
        public ApplicationCommandType CommandType { get; internal set; }

        /// <inheritdoc/>
        public DateTimeOffset CreatedAt
            => SnowflakeUtils.FromSnowflake(this.Id);

        internal RestApplicationCommand(BaseDiscordClient client, ulong id)
            : base(client, id)
        {

        }

        internal static RestApplicationCommand Create(BaseDiscordClient client, Model model, ApplicationCommandType type, ulong guildId = 0)
        {
            if (type == ApplicationCommandType.GlobalCommand)
                return RestGlobalCommand.Create(client, model);

            if (type == ApplicationCommandType.GuildCommand)
                return RestGuildCommand.Create(client, model, guildId);

            return null;
        }

        internal virtual void Update(Model model)
        {
            this.ApplicationId = model.ApplicationId;
            this.Name = model.Name;
            this.Description = model.Description;
            this.DefaultPermission = model.DefaultPermissions.GetValueOrDefault(true);

            this.Options = model.Options.IsSpecified
                ? model.Options.Value.Select(x => ApplicationCommandOption.Create(x)).ToImmutableArray()
                : null;
        }


        /// <inheritdoc/>
        public abstract Task DeleteAsync(RequestOptions options = null);

        IReadOnlyCollection<IApplicationCommandOption> IApplicationCommand.Options => Options;

    }
}
