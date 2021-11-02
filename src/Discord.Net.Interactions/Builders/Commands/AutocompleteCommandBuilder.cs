using System;

namespace Discord.Interactions.Builders
{
    public sealed class AutocompleteCommandBuilder : CommandBuilder<AutocompleteCommandInfo, AutocompleteCommandBuilder, CommandParameterBuilder>
    {
        protected override AutocompleteCommandBuilder Instance => this;

        internal AutocompleteCommandBuilder(ModuleBuilder module) : base(module) { }

        public override AutocompleteCommandBuilder AddParameter(Action<CommandParameterBuilder> configure)
        {
            var parameter = new CommandParameterBuilder(this);
            configure(parameter);
            AddParameters(parameter);
            return this;
        }

        internal override AutocompleteCommandInfo Build(ModuleInfo module, InteractionService commandService) =>
            new AutocompleteCommandInfo(this, module, commandService);
    }
}
