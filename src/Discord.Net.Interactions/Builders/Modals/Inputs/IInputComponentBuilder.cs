using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders
{
    public interface IInputComponentBuilder
    {
        public ModalBuilder Modal { get; }
        public string CustomId { get; }
        public string Label { get; }
        public bool IsRequired { get; }
        public ComponentType ComponentType { get; }
        public Type Type { get; }
        public IReadOnlyCollection<Attribute> Attributes { get; }

        public IInputComponentBuilder WithCustomId(string customId);

        public IInputComponentBuilder WithLabel(string label);

        public IInputComponentBuilder SetIsRequired(bool isRequired);

        public IInputComponentBuilder WithType(Type type);

        public IInputComponentBuilder WithAttributes(params Attribute[] attributes);
    }
}
