namespace Discord.Interactions.Builders
{
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
        ///     Gets or sets the default value of the text input.
        /// </summary>
        public string Value { get; set; }

        public TextInputComponentBuilder(ModalBuilder modal) : base(modal) { }

        public TextInputComponentBuilder WithStyle(TextInputStyle style)
        {
            Style = style;
            return this;
        }

        public TextInputComponentBuilder WithPlaceholder(string placeholder)
        {
            Placeholder = placeholder;
            return this;
        }

        public TextInputComponentBuilder WithMinLenght(int minLenght)
        {
            MinLength = minLenght;
            return this;
        }

        public TextInputComponentBuilder WithMaxLenght(int maxLenght)
        {
            MaxLength = maxLenght;
            return this;
        }

        public TextInputComponentBuilder SetValue(string value)
        {
            Value = value;
            return this;
        }

        internal override TextInputComponentInfo Build(ModalInfo modal) =>
            new(this, modal);
    }
}
