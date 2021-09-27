using System;

namespace Discord.Interactions.Builders
{
    public sealed class CommandParameterBuilder : ParameterBuilder<CommandParameterInfo, CommandParameterBuilder>
    {
        protected override CommandParameterBuilder Instance => this;

        internal CommandParameterBuilder (ICommandBuilder command) : base(command) { }

        public CommandParameterBuilder (ICommandBuilder command, string name, Type type) : base(command, name, type) { }

        internal override CommandParameterInfo Build (ICommandInfo command) =>
            new CommandParameterInfo(this, command);
    }
}
