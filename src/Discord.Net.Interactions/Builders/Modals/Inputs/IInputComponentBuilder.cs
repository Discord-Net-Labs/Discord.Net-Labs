using System;
using System.Collections.Generic;

namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represent a builder for creating <see cref="InputComponentInfo"/>.
    /// </summary>
    public interface IInputComponentBuilder
    {
        /// <summary>
        ///     Gets the parent modal of this input component.
        /// </summary>
        public ModalBuilder Modal { get; }

        /// <summary>
        ///     Gets the custom id of this input component.
        /// </summary>
        public string CustomId { get; }

        /// <summary>
        ///     Gets the label of this input component.
        /// </summary>
        public string Label { get; }

        /// <summary>
        ///     Gets whether this input component is required.
        /// </summary>
        public bool IsRequired { get; }

        /// <summary>
        ///     Gets the component type of this input component.
        /// </summary>
        public ComponentType ComponentType { get; }

        /// <summary>
        ///     Get the reference type of this input component.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///     Gets the default value of this input component.
        /// </summary>
        public object DefaultValue { get; }

        /// <summary>
        ///     Gets a collection of the attributes of this component.
        /// </summary>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        ///     Sets <see cref="CustomId"/>.
        /// </summary>
        /// <param name="customId">New value of the <see cref="CustomId"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public IInputComponentBuilder WithCustomId(string customId);

        /// <summary>
        ///     Sets <see cref="Label"/>.
        /// </summary>
        /// <param name="label">New value of the <see cref="Label"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public IInputComponentBuilder WithLabel(string label);

        /// <summary>
        ///     Sets <see cref="IsRequired"/>.
        /// </summary>
        /// <param name="isRequired">New value of the <see cref="IsRequired"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public IInputComponentBuilder SetIsRequired(bool isRequired);

        /// <summary>
        ///     Sets <see cref="Type"/>.
        /// </summary>
        /// <param name="type">New value of the <see cref="Type"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public IInputComponentBuilder WithType(Type type);

        /// <summary>
        ///     Sets <see cref="DefaultValue"/>.
        /// </summary>
        /// <param name="value">New value of the <see cref="DefaultValue"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public IInputComponentBuilder SetDefaultValue(object value);

        /// <summary>
        ///     Adds attributes to <see cref="Attributes"/>.
        /// </summary>
        /// <param name="attributes">New attributes to be added to <see cref="Attributes"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public IInputComponentBuilder WithAttributes(params Attribute[] attributes);
    }
}
