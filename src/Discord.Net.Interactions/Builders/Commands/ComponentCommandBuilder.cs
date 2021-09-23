using System;

namespace Discord.Interactions.Builders
{
    internal sealed class ComponentCommandBuilder : CommandBuilder<ComponentCommandInfo, ComponentCommandBuilder, CommandParameterBuilder>
    {
        protected override ComponentCommandBuilder Instance => this;

        public ComponentCommandBuilder (ModuleBuilder module) : base(module) { }

        public override ComponentCommandBuilder AddParameter (Action<CommandParameterBuilder> configure)
        {
            var parameter = new CommandParameterBuilder(this);
            configure(parameter);
            AddParameters(parameter);
            return this;
        }

        internal override ComponentCommandInfo Build (ModuleInfo module, InteractionService commandService) =>
            new ComponentCommandInfo(this, module, commandService);
    }
}
