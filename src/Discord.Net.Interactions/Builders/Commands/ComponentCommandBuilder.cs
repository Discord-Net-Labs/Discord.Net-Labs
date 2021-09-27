using System;

namespace Discord.Interactions.Builders
{
    public sealed class ComponentCommandBuilder : CommandBuilder<ComponentCommandInfo, ComponentCommandBuilder, CommandParameterBuilder>
    {
        protected override ComponentCommandBuilder Instance => this;

        internal ComponentCommandBuilder (ModuleBuilder module) : base(module) { }

        public ComponentCommandBuilder (ModuleBuilder module, string name, ExecuteCallback callback) : base(module, name, callback) { }

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
