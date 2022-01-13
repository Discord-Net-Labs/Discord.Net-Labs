namespace Discord.Interactions
{
    public sealed class ModalTextInputAttribute : ModalInputAttribute
    {
        public override ComponentType ComponentType => ComponentType.TextInput;

        /// <summary>
        ///     Gets the style of the text input.
        /// </summary>
        public TextInputStyle Style { get; }

        /// <summary>
        ///     Gets the placeholder of the text input.
        /// </summary>
        public string Placeholder { get; }

        /// <summary>
        ///     Gets the minimum length of the text input.
        /// </summary>
        public int MinLength { get; }

        /// <summary>
        ///     Gets the maximum length of the text input.
        /// </summary>
        public int MaxLength { get; }

        /// <summary>
        ///     Create a new <see cref="ModalTextInputAttribute"/>.
        /// </summary>
        /// <param name="customId"The custom id of the text input.></param>
        /// <param name="style">The style of the text input.</param>
        /// <param name="placeholder">The placeholder of the text input.</param>
        /// <param name="minLength">The minimum length of the text input's content.</param>
        /// <param name="maxLength">The maximum length of the text input's content.</param>
        public ModalTextInputAttribute(string customId, TextInputStyle style = TextInputStyle.Short, string placeholder = null, int minLength = 1, int maxLength = 4000)
            : base(customId)
        {
            Style = style;
            Placeholder = placeholder;
            MinLength = minLength;
            MaxLength = maxLength;
        }
    }
}
