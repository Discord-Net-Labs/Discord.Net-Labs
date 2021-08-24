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
        public ApplicationCommandType Type { get; private set; }

        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public string Description { get; private set; }

        /// <inheritdoc/>
        public bool DefaultPermission { get; private set; }

        /// <summary>
        ///     The options of this command.
        /// </summary>
        public IReadOnlyCollection<RestApplicationCommandOption> Options { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset CreatedAt
            => SnowflakeUtils.FromSnowflake(this.Id);

        internal RestApplicationCommand(BaseDiscordClient client, ulong id)
            : base(client, id)
        {

        }

        internal static RestApplicationCommand Create(BaseDiscordClient client, Model model, ulong? guildId)
        {
            if (guildId.HasValue)
            {
                return RestGuildCommand.Create(client, model, guildId.Value);
            }
            else
            {
                return RestGlobalCommand.Create(client, model);
            }
        }

        internal virtual void Update(Model model)
        {
            this.ApplicationId = model.ApplicationId;
            this.Name = model.Name;
            this.Description = model.Description;
            this.DefaultPermission = model.DefaultPermissions.GetValueOrDefault(true);

            this.Options = model.Options.IsSpecified
                ? model.Options.Value.Select(x => RestApplicationCommandOption.Create(x)).ToImmutableArray()
                : null;
        }

        /// <inheritdoc/>
        public abstract Task DeleteAsync(RequestOptions options = null);

        /// <inheritdoc/>
        public abstract Task ModifyAsync<TArg>(Action<TArg> func, RequestOptions options = null)
            where TArg : ApplicationCommandProperties;

        IReadOnlyCollection<IApplicationCommandOption> IApplicationCommand.Options => Options;
    }
}
