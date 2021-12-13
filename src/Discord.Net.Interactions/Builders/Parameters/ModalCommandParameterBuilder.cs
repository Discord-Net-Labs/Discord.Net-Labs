using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Interactions.Builders
{
    public class ModalCommandParameterBuilder : ParameterBuilder<ModalCommandParameterInfo, ModalCommandParameterBuilder>
    {
        public ModalParameterInitializer ModalParameterInitializer { get; internal set; }

        protected override ModalCommandParameterBuilder Instance => this;

        internal ModalCommandParameterBuilder(ICommandBuilder command) : base(command) { }

        public ModalCommandParameterBuilder(ICommandBuilder command, string name, Type type, ModalParameterInitializer modalParameterInitializer) : base(command, name, type)
        {
            ModalParameterInitializer = modalParameterInitializer;
        }

        internal override ModalCommandParameterInfo Build(ICommandInfo command) =>
            new(this, command);
    }
}
