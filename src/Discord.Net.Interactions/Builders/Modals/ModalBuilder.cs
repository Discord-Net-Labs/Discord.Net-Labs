using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represents a builder for creating <see cref="ModalInfo"/>.
    /// </summary>
    public class ModalBuilder
    {
        private readonly List<TextInputComponentBuilder> _textComponents;

        /// <summary>
        ///     Gets the initialization delegate for this modal.
        /// </summary>
        public ModalInitializer ModalInitializer { get; internal set; }

        /// <summary>
        ///     Gets the title of this modal.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Gets a collection of the text components of this modal.
        /// </summary>
        public IReadOnlyCollection<TextInputComponentBuilder> TextComponents => _textComponents;

        internal ModalBuilder()
        {
            _textComponents = new();
        }

        /// <summary>
        ///     Initializes a new <see cref="ModalBuilder"/>
        /// </summary>
        /// <param name="modalInitializer">The initialization delegate for this modal.</param>
        public ModalBuilder(ModalInitializer modalInitializer) : this()
        {
            ModalInitializer = modalInitializer;
        }

        /// <summary>
        ///     Sets <see cref="Title"/>.
        /// </summary>
        /// <param name="title">New value of the <see cref="Title"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ModalBuilder WithTitle(string title)
        {
            Title = title;
            return this;
        }

        /// <summary>
        ///     Adds text components to <see cref="TextComponents"/>.
        /// </summary>
        /// <param name="configure">Text Component builder factory.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public ModalBuilder AddTextComponent(Action<TextInputComponentBuilder> configure)
        {
            var builder = new TextInputComponentBuilder(this);
            configure(builder);
            _textComponents.Add(builder);
            return this;
        }

        internal ModalInfo Build() => new(this);
    }
}
