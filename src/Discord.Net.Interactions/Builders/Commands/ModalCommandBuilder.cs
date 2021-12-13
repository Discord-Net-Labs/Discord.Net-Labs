using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Interactions.Builders
{
    public class ModalCommandBuilder : CommandBuilder<ModalCommandInfo, ModalCommandBuilder, ModalCommandParameterBuilder>
    {
        protected override ModalCommandBuilder Instance => this;

        public ModalCommandBuilder(ModuleBuilder module) : base(module) { }

        public ModalCommandBuilder(ModuleBuilder module, string name, ExecuteCallback callback) : base(module, name, callback) { }

        public override ModalCommandBuilder AddParameter(Action<ModalCommandParameterBuilder> configure)
        {
            var parameter = new ModalCommandParameterBuilder(this);
            configure(parameter);
            AddParameters(parameter);
            return this;
        }

        internal override ModalCommandInfo Build(ModuleInfo module, InteractionService commandService) =>
            new(this, module, commandService);
    }
}
