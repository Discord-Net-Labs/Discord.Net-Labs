using System;

namespace Discord.Interactions.Builders
{
    public sealed class SlashCommandBuilder : CommandBuilder<SlashCommandInfo, SlashCommandBuilder, SlashCommandParameterBuilder>
    {
        protected override SlashCommandBuilder Instance => this;

        public string Description { get; set; }
        public bool DefaultPermission { get; set; } = true;

        internal SlashCommandBuilder (ModuleBuilder module) : base(module) { }

        public SlashCommandBuilder (ModuleBuilder module, string name, ExecuteCallback callback) : base(module, name, callback) { }

        public SlashCommandBuilder WithDescription (string description)
        {
            Description = description;
            return this;
        }

        public SlashCommandBuilder WithDefaultPermission (bool permission)
        {
            DefaultPermission = permission;
            return Instance;
        }

        public override SlashCommandBuilder AddParameter(Action<SlashCommandParameterBuilder> configure)
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
