using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders
{
    public abstract class InputComponentBuilder<TInfo, TBuilder> : IInputComponentBuilder
        where TInfo : InputComponentInfo
        where TBuilder : InputComponentBuilder<TInfo, TBuilder>
    {
        private readonly List<Attribute> _attributes;
        protected abstract TBuilder Instance { get; }

        public ModalBuilder Modal { get; }
        public string CustomId { get; set; }
        public string Label { get; set; }
        public bool IsRequired { get; set; }
        public ComponentType ComponentType { get; internal set; }
        public Type Type { get; private set; }
        public IReadOnlyCollection<Attribute> Attributes => _attributes;

        public InputComponentBuilder(ModalBuilder modal)
        {
            Modal = modal;
            _attributes = new();
        }

        public TBuilder WithCustomId(string customId)
        {
            CustomId = customId;
            return Instance;
        }

        public TBuilder WithLabel(string label)
        {
            Label = label;
            return Instance;
        }

        public TBuilder SetIsRequired(bool isRequired)
        {
            IsRequired = isRequired;
            return Instance;
        }

        public TBuilder WithComponentType(ComponentType componentType)
        {
            ComponentType = componentType;
            return Instance;
        }

        public TBuilder WithType(Type type)
        {
            Type = type;
            return Instance;
        }

        public TBuilder WithAttributes(params Attribute[] attributes)
        {
            _attributes.AddRange(attributes);
            return Instance;
        }

        internal abstract TInfo Build(ModalInfo modal);

        //IInputComponentBuilder
        IInputComponentBuilder IInputComponentBuilder.WithCustomId(string customId) => WithCustomId(customId);
        IInputComponentBuilder IInputComponentBuilder.WithLabel(string label) => WithCustomId(label);
        IInputComponentBuilder IInputComponentBuilder.WithType(Type type) => WithType(type);
        IInputComponentBuilder IInputComponentBuilder.WithAttributes(params Attribute[] attributes) => WithAttributes(attributes);
        IInputComponentBuilder IInputComponentBuilder.SetIsRequired(bool isRequired) => SetIsRequired(isRequired);
    }
}
