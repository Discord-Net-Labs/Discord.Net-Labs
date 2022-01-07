using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders
{
    public class ModalBuilder
    {
        private readonly List<TextInputComponentBuilder> _textComponents = new();

        public InteractionService InteractionService { get; }
        public ModalInitializer ModalInitializer { get; internal set; }
        public string Title { get; set; }
        public IReadOnlyCollection<TextInputComponentBuilder> TextComponents => _textComponents;

        internal ModalBuilder(InteractionService interactionService)
        {
            InteractionService = interactionService;
        }

        public ModalBuilder(InteractionService interactionService, ModalInitializer modalInitializer) : this(interactionService)
        {
            ModalInitializer = modalInitializer;
        }

        public ModalBuilder WithTitle(string title)
        {
            Title = title;
            return this;
        }

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
