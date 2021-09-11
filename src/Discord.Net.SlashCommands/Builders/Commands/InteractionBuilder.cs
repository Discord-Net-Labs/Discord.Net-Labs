using System;

namespace Discord.SlashCommands.Builders
{
    internal sealed class InteractionBuilder : CommandBuilder<InteractionInfo, InteractionBuilder, CommandParameterBuilder>
    {
        protected override InteractionBuilder Instance => this;

        public InteractionBuilder (ModuleBuilder module) : base(module) { }

        public override InteractionBuilder AddParameter (Action<CommandParameterBuilder> configure)
        {
            var parameter = new CommandParameterBuilder(this);
            configure(parameter);
            AddParameters(parameter);
            return this;
        }

        internal override InteractionInfo Build (ModuleInfo module, SlashCommandService commandService) =>
            new InteractionInfo(this, module, commandService);
    }
}
