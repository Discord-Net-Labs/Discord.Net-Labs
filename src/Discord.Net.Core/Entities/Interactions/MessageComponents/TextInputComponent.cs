namespace Discord
{
    /// <summary>
    ///     Respresents a <see cref="IMessage"/> text input.
    /// </summary>
    public class TextInputComponent : IMessageComponent
    {
        /// <inheritdoc/>
        public ComponentType Type => ComponentType.TextInput;

        /// <inheritdoc/>
        public string CustomId { get; }

        /// <summary>
        ///     Gets the label of the component, this is the text shown above it.
        /// </summary>
        public string Label { get; }

        /// <summary>
        ///     Gets the defualt text of the component. 
        /// </summary>
        public string Placeholder { get; }

        /// <summary>
        ///     Gets the minimum lenght of the inputed text.
        /// </summary>
        public int? MinLength { get; }

        /// <summary>
        ///     Gets the maximum length of the inputed text.
        /// </summary>
        public int? MaxLength { get; }

        /// <summary>
        ///     Gets the style of the component.
        /// </summary>
        public TextInputStyle Style { get; }


        internal TextInputComponent(string customId, string label, string placeholder, int? minLength, int? maxLength, TextInputStyle style)
        {
            CustomId = customId;
            Label = label;
            Placeholder = placeholder;
            MinLength = minLength;
            MaxLength = maxLength;
            Style = style;
        }
    }
}
