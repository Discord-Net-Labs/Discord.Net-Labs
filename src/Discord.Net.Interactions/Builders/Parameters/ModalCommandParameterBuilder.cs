using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Interactions.Builders
{

    /// <summary>
    ///     Represents a builder for creating <see cref="ModalCommandBuilder"/>.
    /// </summary>
    public class ModalCommandParameterBuilder : ParameterBuilder<ModalCommandParameterInfo, ModalCommandParameterBuilder>
    {
        private readonly Dictionary<string, Action<IModal, object>> _textInputComponents = new();

        public IReadOnlyDictionary<string, Action<IModal, object>> TextInputComponents { get; }

        /// <summary>
        ///     Gets the parameters initializer.
        /// </summary>
        public ModalParameterInitializer ModalParameterInitializer { get; internal set; }

        protected override ModalCommandParameterBuilder Instance => this;

        internal ModalCommandParameterBuilder(ICommandBuilder command) : base(command) { }

        /// <summary>
        ///     Creates a new <see cref="ModalCommandParameterBuilder"/>.
        /// </summary>
        /// <param name="command">Parent command of this parameter.</param>
        /// <param name="name">Name of this command.</param>
        /// <param name="type">Type of this parameter.</param>
        /// <param name="modalParameterInitializer">the initializer of this parameter.</param>
        public ModalCommandParameterBuilder(ICommandBuilder command, string name, Type type, ModalParameterInitializer modalParameterInitializer) : base(command, name, type)
        {
            ModalParameterInitializer = modalParameterInitializer;
        }

        public ModalCommandParameterBuilder AddTextInputComponent(string label, Action<IModal, object> propertySetter)
        {
            _textInputComponents[label] = propertySetter;
            return this;
        }

        public ModalCommandParameterBuilder AddTextInputComponents(IDictionary<string, Action<IModal, object>> components)
        {
            foreach(var component in components)
                _textInputComponents[component.Key] = component.Value;

            return this;
        }

        internal override ModalCommandParameterInfo Build(ICommandInfo command) =>
            new(this, command);
    }
}
