using System;

namespace Discord.Interactions
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public abstract class ModalInputAttribute : Attribute
    {
        /// <summary>
        ///     Gets the custom id of the text input.
        /// </summary>
        public string CustomId { get; }

        /// <summary>
        ///     Gets the type of the component.
        /// </summary>
        public abstract ComponentType ComponentType { get; }

        /// <summary>
        ///     Gets the label of the input.
        /// </summary>
        public string Label { get; }

        /// <summary>
        ///     Gets whether the user is required to input a value.
        /// </summary>
        public bool Required { get; }

        /// <summary>
        ///     Create a new <see cref="ModalInputAttribute"/>.
        /// </summary>
        /// <param name="label">The label of the input.</param>
        /// <param name="customId">The custom id of the input.</param>
        /// <param name="required">Whether the user is required to input a value.></param>
        protected ModalInputAttribute(string label, string customId, bool required = true)
        {
            CustomId = customId;
            Label = label;
            Required = required;
        }
    }
}
