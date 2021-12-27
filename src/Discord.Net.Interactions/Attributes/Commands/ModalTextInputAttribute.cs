using System;

namespace Discord.Interactions
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ModalTextInputAttribute : Attribute
    {
        /// <summary>
        ///     Gets the custom id of the text input.
        /// </summary>
        public string CustomId { get; }

        /// <summary>
        ///     Gets the style of the text input.
        /// </summary>
        public TextInputStyle Style { get; }

        /// <summary>
        ///     Gets the label of the text input.
        /// </summary>
        public string Label { get; }

        /// <summary>
        ///     Gets the placeholder of the text input.
        /// </summary>
        public string Placeholder { get; }

        /// <summary>
        ///     Gets the minimum length of the text input.
        /// </summary>
        public int? MinLength { get; }

        /// <summary>
        ///     Gets the maximum length of the text input.
        /// </summary>
        public int? MaxLength { get; }

        /// <summary>
        ///     Gets whether the user is required to input text.
        /// </summary>
        public bool? Required { get; }

        /// <summary>
        ///     Gets or sets the default value of the text input.
        /// </summary>
        public string Value { get; }

        /// <summary>
        ///     Create a new <see cref="ModalTextInputAttribute"/>.
        /// </summary>
        /// <param name="label">The label of the text input.</param>
        /// <param name="customId"The custom id of the text input.></param>
        /// <param name="style">The style of the text input.</param>
        /// <param name="placeholder">The placeholder of the text input.</param>
        /// <param name="minLength">The minimum length of the text input's content.</param>
        /// <param name="maxLength">The maximum length of the text input's content.</param>
        /// <param name="required">Whether the user is required to input text.></param>
        /// <param name="value">The default value of the text input,</param>
        public ModalTextInputAttribute(string label, string customId,
            TextInputStyle style = TextInputStyle.Short, string placeholder = null, int minLength = 1,
            int maxLength = 4000, bool required = true, string value = null)
        {
            CustomId = customId;
            Style = style;
            Label = label;
            Placeholder = placeholder;
            MinLength = minLength;
            MaxLength = maxLength;
            Required = required;
            Value = value;
        }
    }
}
