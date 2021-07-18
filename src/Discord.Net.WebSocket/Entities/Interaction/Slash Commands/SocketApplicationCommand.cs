using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.ApplicationCommand;

namespace Discord.WebSocket
{
    /// <summary>
    /// Represents a Web-Socket based <see cref="IApplicationCommand"/>
    /// </summary>
    public class SocketApplicationCommand : SocketEntity<ulong>, IApplicationCommand
    {
        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public string Description { get; private set; }

        /// <inheritdoc cref="IApplicationCommand.ApplicationId"/>
        public ulong ApplicationId { get; }

        /// <inheritdoc/>
        public bool DefaultPermission { get; private set; }

        /// <inheritdoc cref="IApplicationCommand.Guild"/>
        public SocketGuild Guild { get; }

        /// <summary>
        /// Wheter this command is a Global Command or a Guild Command
        /// </summary>
        public bool IsGlobal => Guild == null;

        /// <inheritdoc cref="IApplicationCommand.Options"/>
        public IReadOnlyList<ApplicationCommandOption> Options { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

        /// <inheritdoc/>
        ulong IApplicationCommand.ApplicationId => ApplicationId;

        /// <inheritdoc/>
        IGuild IApplicationCommand.Guild => Guild;

        /// <inheritdoc/>
        IEnumerable<IApplicationCommandOption> IApplicationCommand.Options => Options;

        internal SocketApplicationCommand (DiscordSocketClient discord, ulong id, SocketGuild guild, Model model) : base(discord, id)
        {
            Name = model.Name;
            Guild = guild;
            ApplicationId = model.ApplicationId;

            Update(model);
        }

        internal void Update ( Model model )
        {
            Name = model.Name;
            Description = model.Description;

            if(model.DefaultPermission.IsSpecified)
                DefaultPermission = model.DefaultPermission.Value;

            if(model.Options.IsSpecified)
                Options = model.Options.Value.Select(x => ApplicationCommandOption.Create(x, ApplicationCommandOption.MaxOptionDepth)).ToList();
        }

        /// <inheritdoc cref="IApplicationCommand.Modify(string, string, bool, IEnumerable{IApplicationCommandOption}, RequestOptions)"/>
        public async Task<RestApplicationCommand> Modify (string name, string description, bool defaultPermission,
            IEnumerable<IApplicationCommandOption> commandOptions, RequestOptions options) =>
            await InteractionHelper.ModifyApplicationCommand(Discord, Id, Guild, name, description, defaultPermission, commandOptions, options)
            .ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task UpdateAsync (RequestOptions options = null)
        {
            Model model;

            if (IsGlobal)
                model = await Discord.ApiClient.GetGlobalApplicationCommand( Id, options).ConfigureAwait(false);
            else
                model = await Discord.ApiClient.GetGuildApplicationCommand( Guild.Id, Id, options).ConfigureAwait(false);

            Update(model);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync (RequestOptions options = null) =>
            await InteractionHelper.DeleteApplicationCommand(Discord, Id, Guild, options).ConfigureAwait(false);

        /// <inheritdoc cref="IApplicationCommand.ModifyPermissions(IEnumerable{ApplicationCommandPermission}, RequestOptions)"/>
        public async Task<GuildApplicationCommandPermission> ModifyPermissions (IEnumerable<ApplicationCommandPermission> perms, RequestOptions options = null) =>
            await InteractionHelper.ModifyCommandPermissions(Discord, this, perms, options).ConfigureAwait(false);

        /// <inheritdoc/>
        async Task<IApplicationCommand> IApplicationCommand.Modify (string name, string description, bool defaultPermission,
            IEnumerable<IApplicationCommandOption> commandOptions, RequestOptions options) =>
            await Modify(name, description, defaultPermission, commandOptions, options);

        /// <inheritdoc/>
        async Task<IApplicationCommandPermission> IApplicationCommand.ModifyPermissions (IEnumerable<ApplicationCommandPermission> perms, RequestOptions options) =>
            await ModifyPermissions(perms, options);
    }
}
