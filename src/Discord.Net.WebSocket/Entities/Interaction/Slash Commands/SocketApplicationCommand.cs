using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Gateway.ApplicationCommandCreatedUpdatedEvent;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represends a Websocket-based <see cref="IApplicationCommand"/> recieved over the gateway.
    /// </summary>
    public class SocketApplicationCommand : SocketEntity<ulong>, IApplicationCommand
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
        ///     A collection of <see cref="ApplicationCommandOption"/>'s recieved over the gateway.
        /// </summary>
        public IReadOnlyCollection<ApplicationCommandOption> Options { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset CreatedAt
            => SnowflakeUtils.FromSnowflake(this.Id);

        /// <summary>
        ///     The <see cref="SocketGuild"/> where this application was created.
        /// </summary>
        public SocketGuild Guild
            => Discord.GetGuild(this.GuildId);
        private ulong GuildId { get; set; }

        internal SocketApplicationCommand(DiscordSocketClient client, ulong id)
            : base(client, id)
        {

        }
        internal static SocketApplicationCommand Create(DiscordSocketClient client, Model model)
        {
            var entity = new SocketApplicationCommand(client, model.Id);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            this.ApplicationId = model.ApplicationId;
            this.Description = model.Description;
            this.Name = model.Name;
            this.GuildId = model.GuildId;
            this.DefaultPermission = model.DefaultPermission.GetValueOrDefault(true);


            this.Options = model.Options.IsSpecified
                ? model.Options.Value.Select(x => ApplicationCommandOption.Create(x)).ToImmutableArray()
                : new ImmutableArray<ApplicationCommandOption>();
        }

        /// <inheritdoc/>
        public Task DeleteAsync(RequestOptions options = null)
            => InteractionHelper.DeleteGuildCommand(Discord, this.GuildId, this, options);

        IReadOnlyCollection<IApplicationCommandOption> IApplicationCommand.Options => Options;
    }
}
