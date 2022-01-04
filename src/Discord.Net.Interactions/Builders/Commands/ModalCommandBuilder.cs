using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represents a builder for creating a <see cref="ModalCommandInfo"/>.
    /// </summary>
    public class ModalCommandBuilder : CommandBuilder<ModalCommandInfo, ModalCommandBuilder, ModalCommandParameterBuilder>
    {
        private readonly Dictionary<string, Action<IModal, object>> _textInputComponents = new();

        public IReadOnlyDictionary<string, Action<IModal, object>> TextInputComponents { get; }

        protected override ModalCommandBuilder Instance => this;

        /// <summary>
        ///     Initializes a new <see cref="ModalCommandBuilder"/>.
        /// </summary>
        /// <param name="module">Parent module of this modal.</param>
        public ModalCommandBuilder(ModuleBuilder module) : base(module) { }

        /// <summary>
        ///     Initializes a new <see cref="ModalCommandBuilder"/>.
        /// </summary>
        /// <param name="module">Parent module of this modal.</param>
        /// <param name="name">Name of this modal.</param>
        /// <param name="callback">Execution callback of this modal.</param>
        public ModalCommandBuilder(ModuleBuilder module, string name, ExecuteCallback callback) : base(module, name, callback) { }

        /// <summary>
        ///     Adds a modal parameter to the parameters collection.
        /// </summary>
        /// <param name="configure"><see cref="ModalCommandParameterBuilder"/> factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public override ModalCommandBuilder AddParameter(Action<ModalCommandParameterBuilder> configure)
        {
            var parameter = new ModalCommandParameterBuilder(this);
            configure(parameter);
            AddParameters(parameter);
            return this;
        }

        public ModalCommandBuilder AddTextInputComponent(string label, Action<IModal, object> propertySetter)
        {
            _textInputComponents[label] = propertySetter;
            return this;
        }

        internal override ModalCommandInfo Build(ModuleInfo module, InteractionService commandService) =>
            new(this, module, commandService);
    }
}
