using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Interactions
{
    public abstract class InputComponentInfo
    {
        public ModalInfo Modal { get; }
        public string CustomId { get; }
        public string Label { get; }
        public bool IsRequired { get; }
        public ComponentType ComponentType { get; }
        public Type Type { get; }
        public IReadOnlyCollection<Attribute> Attributes { get; }

        protected InputComponentInfo(Builders.IInputComponentBuilder builder, ModalInfo modal)
        {
            Modal = modal;
            CustomId = builder.CustomId;
            Label = builder.Label;
            IsRequired = builder.IsRequired;
            ComponentType = builder.ComponentType;
            Type = builder.Type;
            Attributes = builder.Attributes.ToImmutableArray();
        }
    }
}
