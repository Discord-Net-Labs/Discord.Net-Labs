namespace Discord.Interactions.Builders
{
    /// <summary>
    ///     Represents a builder for creating <see cref="TextInputComponentInfo"/>.
    /// </summary>
    public class TextInputComponentBuilder : InputComponentBuilder<TextInputComponentInfo, TextInputComponentBuilder>
    {
        protected override TextInputComponentBuilder Instance => this;

        /// <summary>
        ///     Gets the style of the text input.
        /// </summary>
        public TextInputStyle Style { get; set; }

        /// <summary>
        ///     Gets the placeholder of the text input.
        /// </summary>
        public string Placeholder { get; set; }

        /// <summary>
        ///     Gets the minimum length of the text input.
        /// </summary>
        public int MinLength { get; set; }

        /// <summary>
        ///     Gets the maximum length of the text input.
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        ///     Initializes a new <see cref="TextInputComponentBuilder"/>.
        /// </summary>
        /// <param name="modal">Parent modal of this component.</param>
        public TextInputComponentBuilder(ModalBuilder modal) : base(modal) { }

        /// <summary>
        ///     Sets <see cref="Style"/>.
        /// </summary>
        /// <param name="style">New value of the <see cref="SetValue(string)"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public TextInputComponentBuilder WithStyle(TextInputStyle style)
        {
            Style = style;
            return this;
        }

        /// <summary>
        ///     Sets <see cref="Placeholder"/>.
        /// </summary>
        /// <param name="placeholder">New value of the <see cref="Placeholder"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public TextInputComponentBuilder WithPlaceholder(string placeholder)
        {
            Placeholder = placeholder;
            return this;
        }

        /// <summary>
        ///     Sets <see cref="MinLength"/>.
        /// </summary>
        /// <param name="minLenght">New value of the <see cref="MinLength"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public TextInputComponentBuilder WithMinLenght(int minLenght)
        {
            MinLength = minLenght;
            return this;
        }

        /// <summary>
        ///     Sets <see cref="MaxLength"/>.
        /// </summary>
        /// <param name="maxLenght">New value of the <see cref="MaxLength"/>.</param>
        /// <returns>
        ///     The builder instance.
        /// </returns>
        public TextInputComponentBuilder WithMaxLenght(int maxLenght)
        {
            MaxLength = maxLenght;
            return this;
        }

        internal override TextInputComponentInfo Build(ModalInfo modal) =>
            new(this, modal);
    }
}
