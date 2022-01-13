using System;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represents a builder for creating <see cref="SlashCommandInfo"/>.
    /// </summary>
    public sealed class SlashCommandBuilder : CommandBuilder<SlashCommandInfo, SlashCommandBuilder, SlashCommandParameterBuilder>
    {
        protected override SlashCommandBuilder Instance => this;

        /// <summary>
        ///     Gets and sets the description of this command.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets and sets the default permission of this command.
        /// </summary>
        public bool DefaultPermission { get; set; } = true;

        /// <summary>
        ///     Gets whether this command can be used in DMs.
        /// </summary>
        public bool IsEnabledInDm { get; set; } = true;

        /// <summary>
        ///     Gets the default permissions needed for executing this command.
        /// </summary>
        public GuildPermission? DefaultMemberPermission { get; set; } = null;

        internal SlashCommandBuilder (ModuleBuilder module) : base(module) { }

        /// <summary>
        ///     Initializes a new <see cref="SlashCommandBuilder"/>.
        /// </summary>
        /// <param name="module">Parent module of this command.</param>
        /// <param name="name">Name of this command.</param>
        /// <param name="callback">Execution callback of this command.</param>
        public SlashCommandBuilder (ModuleBuilder module, string name, ExecuteCallback callback) : base(module, name, callback) { }

        /// <summary>
        ///     Sets <see cref="Description"/>.
        /// </summary>
        /// <param name="description">New value of the <see cref="Description"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public SlashCommandBuilder WithDescription (string description)
        {
            Description = description;
            return this;
        }

        /// <summary>
        ///     Sets <see cref="DefaultPermission"/>.
        /// </summary>
        /// <param name="defaultPermision">New value of the <see cref="DefaultPermission"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public SlashCommandBuilder WithDefaultPermission (bool permission)
        {
            DefaultPermission = permission;
            return Instance;
        }

        /// <summary>
        ///     Sets <see cref="IsEnabledInDm"/>.
        /// </summary>
        /// <param name="isEnabled">New value of the <see cref="IsEnabledInDm"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public SlashCommandBuilder SetEnabledInDm(bool isEnabled)
        {
            IsEnabledInDm = isEnabled;
            return this;
        }

        /// <summary>
        ///     Sets <see cref="DefaultMemberPermission"/>.
        /// </summary>
        /// <param name="permissions">New value of the <see cref="DefaultMemberPermission"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public SlashCommandBuilder WithDefaultMemberPermission(GuildPermission permissions)
        {
            DefaultMemberPermission = permissions;
            return this;
        }

        /// <summary>
        ///     Adds a command parameter to the parameters collection.
        /// </summary>
        /// <param name="configure"><see cref="SlashCommandParameterBuilder"/> factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public override SlashCommandBuilder AddParameter (Action<SlashCommandParameterBuilder> configure)
        {
            var parameter = new SlashCommandParameterBuilder(this);
            configure(parameter);
            AddParameters(parameter);
            return this;
        }

        internal override SlashCommandInfo Build (ModuleInfo module, InteractionService commandService) =>
            new SlashCommandInfo(this, module, commandService);
    }
}
