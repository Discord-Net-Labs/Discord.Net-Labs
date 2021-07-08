using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.ApplicationCommand;

namespace Discord.Rest
{
    /// <summary>
    /// Represents a REST-based Application Command
    /// </summary>
    public class RestApplicationCommand : RestEntity<ulong>, IApplicationCommand
    {
        /// <inheritdoc/>
        public string Name { get; private set; }
        /// <inheritdoc/>
        public string Description { get; private set; }
        /// <inheritdoc/>
        public ulong ApplicationId { get; }
        /// <inheritdoc/>
        public bool DefaultPermission { get; private set; }
        /// <inheritdoc cref="IApplicationCommand.Guild"/>
        public IGuild Guild { get; }

        /// <summary>
        /// Wheter the command is a Global Command or a Guild Command
        /// </summary>
        public bool IsGlobal => Guild == null;

        /// <inheritdoc cref="IApplicationCommand.Options"/>
        public IReadOnlyList<IApplicationCommandOption> Options { get; private set; }
        /// <inheritdoc/>
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

        /// <inheritdoc/>
        IEnumerable<IApplicationCommandOption> IApplicationCommand.Options => Options;
        /// <inheritdoc/>
        IGuild IApplicationCommand.Guild => Guild;

        internal RestApplicationCommand (BaseDiscordClient discord, ulong id, IGuild guild, Model model) : base(discord, id)
        {
            Guild = guild;
            ApplicationId = model.ApplicationId;

            Update(model);
        }

        internal void Update (Model model)
        {
            Name = model.Name;
            Description = model.Description;
            if (model.DefaultPermission.IsSpecified)
                DefaultPermission = model.DefaultPermission.Value;
            if (model.Options.IsSpecified)
                Options = model.Options.Value.Select(x => ApplicationCommandOption.Create(x, ApplicationCommandOption.MaxOptionDepth)).ToList();
        }

        /// <inheritdoc/>
        public async Task UpdateAsync (RequestOptions options = null)
        {
            Model model;

            if (IsGlobal)
                model = await Discord.ApiClient.GetGlobalApplicationCommand(ApplicationId, Id, options).ConfigureAwait(false);
            else
                model = await Discord.ApiClient.GetGuildApplicationCommand(ApplicationId, Guild.Id, Id, options).ConfigureAwait(false);

            Update(model);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync (RequestOptions options = null) =>
            await SlashCommandHelper.DeleteApplicationCommand(Discord, Id, Guild, options).ConfigureAwait(false);

        /// <inheritdoc cref="IApplicationCommand.Modify(string, string, bool, IEnumerable{IApplicationCommandOption}, RequestOptions)"/>
        public async Task<RestApplicationCommand> Modify (string name, string description, bool defaultPermission = true,
            IEnumerable<IApplicationCommandOption> commandOptions = null, RequestOptions options = null) =>
            await SlashCommandHelper.ModifyApplicationCommand(Discord, Id, Guild, name, description, defaultPermission, commandOptions, options)
            .ConfigureAwait(false);

        /// <inheritdoc/>
        async Task<IApplicationCommand> IApplicationCommand.Modify (string name, string description, bool defaultPermission,
            IEnumerable<IApplicationCommandOption> commandOptions, RequestOptions options) => await Modify(name,description, defaultPermission, commandOptions, options);
    }
}
