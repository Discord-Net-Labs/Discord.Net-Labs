using System;

namespace Discord.Interactions.Builders
{
    internal class ContextCommandBuilder : CommandBuilder<ContextCommandInfo, ContextCommandBuilder, CommandParameterBuilder>
    {
        protected override ContextCommandBuilder Instance => this;

        public ApplicationCommandType CommandType { get; set; }
        public bool DefaultPermission { get; set; } = true;

        internal ContextCommandBuilder (ModuleBuilder module) : base(module) { }

        public ContextCommandBuilder SetType (ApplicationCommandType commandType)
        {
            CommandType = commandType;
            return this;
        }

        public ContextCommandBuilder SetDefaultPermission (bool defaultPermision)
        {
            DefaultPermission = defaultPermision;
            return this;
        }

        public override ContextCommandBuilder AddParameter (Action<CommandParameterBuilder> configure)
        {
            var parameter = new CommandParameterBuilder(this);
            configure(parameter);
            AddParameters(parameter);
            return this;
        }

        internal override ContextCommandInfo Build (ModuleInfo module, InteractionService commandService) =>
            ContextCommandInfo.Create(this, module, commandService);
    }
}
