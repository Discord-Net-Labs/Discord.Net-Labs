using System;

namespace Discord.Interactions.Builders
{
    internal sealed class CommandParameterBuilder : ParameterBuilder<CommandParameterInfo, CommandParameterBuilder>
    {
        public CommandParameterBuilder( ICommandBuilder builder) : base( builder) { }

        protected override Builders.CommandParameterBuilder Instance => this;

        public override CommandParameterInfo Build (ICommandInfo command) =>
            new CommandParameterInfo(this, command);
    }
}
