using System;

namespace Discord.Interactions.Builders
{

    /// <summary>
    ///     Represents a builder for creating <see cref="ModalCommandBuilder"/>.
    /// </summary>
    public class ModalCommandParameterBuilder : ParameterBuilder<ModalCommandParameterInfo, ModalCommandParameterBuilder>
    {
        protected override ModalCommandParameterBuilder Instance => this;

        public ModalInfo Modal { get; private set; }

        public bool IsModalParameter => Modal is not null;

        internal ModalCommandParameterBuilder(ICommandBuilder command) : base(command) { }

        public ModalCommandParameterBuilder(ICommandBuilder command, string name, Type type) : base(command, name, type) { }

        public override ModalCommandParameterBuilder SetParameterType(Type type)
        {
            if (typeof(IModal).IsAssignableFrom(type))
                Modal = ModalUtils.GetOrAdd(type);

            return base.SetParameterType(type);
        }

        internal override ModalCommandParameterInfo Build(ICommandInfo command) =>
            new(this, command);
    }
}
